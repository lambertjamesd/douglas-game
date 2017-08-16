using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnChoice
{
    public int bid;
    public Card card;
    public Card extraCard;

    public TurnChoice(int bid, Card card)
    {
        this.bid = bid;
        this.card = card;
    }

    public static TurnChoice Fold()
    {
        return new TurnChoice(0, null);
    }
}

public abstract class PlayerLogic {
    protected int index = 0;
    public PlayerHand hand = null;
    public int money = 1000;
    public Text moneyLabel;

    public PlayerLogic(int index, PlayerHand hand, Text moneyLabel)
    {
        this.index = index;
        this.hand = hand;
        this.moneyLabel = moneyLabel;
        UpdateLabel();
    }

    public void UpdateLabel()
    {
        moneyLabel.text = "$" + money.ToString();
    }

    public void AdjustMoney(int amount)
    {
        money += amount;
        UpdateLabel();
    }

    public bool CanPlayExtraCard(Card next)
    {
        return hand.GetPlayedCards().TrueForAll(card => card.suite == next.suite) && hand.GetPlayedCards().Count == 2;
    }

    public void ExecuteTurn(TurnChoice turn)
    {
        if (turn.card != null)
        {
            hand.PlayCard(turn.card);
            if (turn.extraCard != null)
            {
                hand.PlayCard(turn.extraCard);
            }
            AdjustMoney(-turn.bid);
        }
    }

    public abstract void StartTurn(List<List<Card>> onBoard, int currentBid, int inPot);

    public abstract bool TakingTurn();

    public abstract TurnChoice TakeTurn();
}

public class HumanPlayerLogic : PlayerLogic
{
    private Button foldButton;
    private Button bidButton;
    private NumberSpinner spinner;
    private IEnumerable<Button> cardSelection;
    private TurnChoice choice = null;
    private int cardChoice = -1;
    private bool takingTurn = false;

    public HumanPlayerLogic(
        int index, 
        PlayerHand hand,
        Button foldButton,
        Button bidButton,
        NumberSpinner spinner,
        List<Button> cardSelection, 
        Text moneyLabel
    ) : base(index, hand, moneyLabel)  {
        this.foldButton = foldButton;
        this.bidButton = bidButton;
        this.spinner = spinner;
        this.cardSelection = cardSelection;

        foldButton.onClick.AddListener(() =>
        {
            if (takingTurn)
            {
                choice = TurnChoice.Fold();
            }
        });

        for (int i = 0; i < cardSelection.Count; ++i)
        {
            BindCardClick(cardSelection[i], i);
        }
    }

    private void BindCardClick(Button button, int index)
    {
        button.onClick.AddListener(() =>
        {
            Card pickedCard = index < hand.GetHand().Count ? hand.GetHand()[index] : null;

            if (takingTurn && spinner.value > 0 && pickedCard != null)
            {
                if (choice == null)
                {
                    choice = new TurnChoice(spinner.value, pickedCard);
                }
                else if (choice.card != pickedCard)
                {
                    choice.bid = spinner.value;
                    choice.extraCard = pickedCard;
                }
            }
        });
    }

    public override void StartTurn(List<List<Card>> onBoard, int currentBid, int inPot)
    {
        takingTurn = true;
        choice = null;
        cardChoice = -1;
    }

    public override bool TakingTurn()
    {
        return choice == null || (CanPlayExtraCard(choice.card) && choice.extraCard == null);
    }

    public override TurnChoice TakeTurn()
    {
        takingTurn = false;
        return choice;
    }
}

public class DumbAIPlayer : PlayerLogic
{
    TurnChoice choice = null;
    int pickedCard;

    public DumbAIPlayer(int index, PlayerHand hand, Text moneyLabel) : base(index, hand, moneyLabel)
    {
    }

    private Card PickCard()
    {
        while (hand.GetHand()[pickedCard] == null)
        {
            ++pickedCard;
        }

        Card result = hand.GetHand()[pickedCard];

        ++pickedCard;

        return result;
    }

    public override void StartTurn(List<List<Card>> onBoard, int currentBid, int inPot)
    {
        if (onBoard[index].Count == 0)
        {
            pickedCard = 0;
        }

        choice = new TurnChoice(Math.Max(currentBid, 10), PickCard());
        
        if (CanPlayExtraCard(choice.card))
        {
            choice.extraCard = PickCard();
        }
    }

    public override TurnChoice TakeTurn()
    {
        return choice;
    }

    public override bool TakingTurn()
    {
        return false;
    }
}