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
        return next != null && hand.GetPlayedCards().TrueForAll(card => card.suite == next.suite) && hand.GetPlayedCards().Count == 2;
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
        }
    }

    public abstract IEnumerator StartTurn(List<List<Card>> onBoard, int currentBid, int inPot);
    public abstract TurnChoice TurnResult();
}

public class HumanPlayerLogic : PlayerLogic
{
    private Button foldButton;
    private Transform guiTransform;
    private NumberSpinner spinner;
    private IEnumerable<Button> cardSelection;
    private TurnChoice choice = null;
    private int cardChoice = -1;
    private bool takingTurn = false;
    private Card chosenCard;

    public HumanPlayerLogic(
        int index, 
        PlayerHand hand,
        Button foldButton,
        Transform guiTransform,
        NumberSpinner spinner,
        List<Button> cardSelection, 
        Text moneyLabel
    ) : base(index, hand, moneyLabel)  {
        this.foldButton = foldButton;
        this.guiTransform = guiTransform;
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
        SetGUIVisible(false, -1);
    }

    private void PositionGUI(int slotOffset = 0)
    {
        guiTransform.position = hand.boardSlots[hand.GetPlayedCards().Count + slotOffset].transform.position;
    }

    private void SetGUIVisible(bool value, int useBid)
    {
        guiTransform.gameObject.SetActive(value);
        foldButton.gameObject.SetActive(value);

        PositionGUI(0);

        spinner.SetEnabled(useBid == -1);
        if (useBid != -1)
        {
            spinner.SetValue(useBid);
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
                    chosenCard = pickedCard;
                }
                else if (choice.card != pickedCard)
                {
                    choice.bid = spinner.value;
                    choice.extraCard = pickedCard;
                    chosenCard = pickedCard;
                }
            }
        });
    }

    public override IEnumerator StartTurn(List<List<Card>> onBoard, int currentBid, int inPot)
    {
        takingTurn = true;
        choice = null;
        cardChoice = -1;

        int min = Math.Max(Math.Min((currentBid == -1) ? inPot / 2 : currentBid, money), 1);

        spinner.SetMin(min);
        spinner.SetMax(money);

        SetGUIVisible(true, currentBid);

        while (choice == null || (CanPlayExtraCard(choice.card) && choice.extraCard == null))
        {
            if (chosenCard != null)
            {
                PositionGUI(1);
                yield return hand.PutCardOnTable(chosenCard);
                chosenCard = null;
            }

            yield return null;
        }

        SetGUIVisible(false, -1);

        if (chosenCard != null)
        {
            yield return hand.PutCardOnTable(chosenCard);
            chosenCard = null;
        }

        takingTurn = false;
    }

    public override TurnChoice TurnResult()
    {
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

    public override IEnumerator StartTurn(List<List<Card>> onBoard, int currentBid, int inPot)
    {
        if (onBoard[index].Count == 0)
        {
            pickedCard = 0;
        }

        int bid = (currentBid == -1) ? Math.Max(10, inPot / 2) : currentBid;

        choice = new TurnChoice(Math.Min(bid, money), PickCard());

        yield return hand.PutCardOnTable(choice.card);
        
        if (CanPlayExtraCard(choice.card))
        {
            choice.extraCard = PickCard();

            yield return hand.PutCardOnTable(choice.extraCard);
        }

        yield return null;
    }

    public override TurnChoice TurnResult()
    {
        return choice;
    }
}