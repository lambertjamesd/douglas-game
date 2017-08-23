using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

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

    public IEnumerable<Card> PlayedCards()
    {
        if (card != null)
        {
            yield return card;
        }

        if (extraCard != null)
        {
            yield return extraCard;
        }
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
            if (chosenCard != choice.extraCard)
            {
                yield return hand.PutCardOnTable(chosenCard);
            }
            chosenCard = null;
        }

        takingTurn = false;
    }

    public override TurnChoice TurnResult()
    {
        return choice;
    }
}

public class CardAIBase : PlayerLogic
{
    TurnChoice choice = null;
    IEnumerator<Card> pickedCards;
    int handScore = 0;

    int[] bidAmount = new int[]{
        5,
        10,
        20,
        30,
        40,
        50,
        100,
        200,
        300,
        400,
        500,
    };

    public CardAIBase(int index, PlayerHand hand, Text moneyLabel)
        : base(index, hand, moneyLabel)
    {
    }

    private Card PickCard()
    {
        if (pickedCards.MoveNext())
        {
            return pickedCards.Current;
        }
        else
        {
            return null;
        }
    }

    public virtual float WinProbability(int bid, int score)
    {
        return 1.0f;
    }

    public virtual float OponentFoldProbability()
    {
        return 0.0f;
    }

    public IEnumerable<Card> PlayOrderCommon(IEnumerable<Card> cardsToPlay)
    {
        return PlayOrder(cardsToPlay.Take(3)).Concat(cardsToPlay.Skip(3));
    }

    public virtual IEnumerable<Card> PlayOrder(IEnumerable<Card> cardsToPlay)
    {
        return cardsToPlay;
    }

    public override IEnumerator StartTurn(List<List<Card>> onBoard, int currentBid, int inPot)
    {
        if (onBoard[index].Count == 0)
        {
            var pickedCardsList = PlayOrderCommon(IdealHand(hand.GetHand(), new Card[] { }));
            pickedCards = pickedCardsList.GetEnumerator();
            handScore = ScoreHand(pickedCardsList);
        }

        int minBid = inPot / 2;
        int bid = currentBid;
        bool shouldFold = false;

        if (currentBid == -1)
        {
            float expectedWinnings = ScoreOutcome(WinProbability(bid, handScore), OponentFoldProbability(), inPot, minBid);
            bid = minBid;
            foreach (int bidCheck in bidAmount)
            {
                if (bidCheck > minBid)
                {
                    float winnings = ScoreOutcome(WinProbability(bid, handScore), OponentFoldProbability(), inPot, bidCheck);

                    if (winnings > expectedWinnings)
                    {
                        expectedWinnings = winnings;
                        bid = bidCheck;
                    }
                }
            }

            shouldFold = expectedWinnings < 0.0f;
        }
        else
        {
            shouldFold = ScoreOutcome(WinProbability(bid, handScore), OponentFoldProbability(), inPot, bid) < 0.0f;
        }

        choice = shouldFold ? TurnChoice.Fold() : new TurnChoice(Math.Min(bid, money), PickCard());

        yield return hand.PutCardOnTable(choice.card);
        
        if (CanPlayExtraCard(choice.card))
        {
            choice.extraCard = PickCard();
        }

        yield return null;
    }

    public override TurnChoice TurnResult()
    {
        return choice;
    }

    public static Suite SuiteForCards(IEnumerable<Card> cards)
    {
        Suite result = Suite.Count;

        foreach (Card card in cards)
        {
            if (result == Suite.Count)
            {
                result = card.suite;
            }
            else if (result != card.suite)
            {
                return Suite.Count;
            }
        }

        return result;
    }

    public static IEnumerable<Card> IdealHand(IEnumerable<Card> cards, IEnumerable<Card> playedCards)
    {
        int[] suiteCount = new int[]{
            0, 0, 0, 0
        };

        var cardList = cards.ToList();
        cardList.Sort((a, b) => b.PointValue() - a.PointValue());

        Suite suiteToUse = SuiteForCards(playedCards);

        for (int i = 0; suiteToUse == Suite.Count && i < cardList.Count; ++i)
        {
            Card card = cardList[i];
            if (suiteCount[(int)card.suite] == 2)
            {
                suiteToUse = card.suite;
                break;
            }
            else
            {
                suiteCount[(int)card.suite]++;
            }
        }

        int suitePoints = 0;
        int suiteCardsPlayed = 0;
        int notSuiteHighCard = 0;
        int tripleCardScore = 0;
        Card extraCard = null;

        cardList.InsertRange(0, playedCards);

        for (int i = 0; i < cardList.Count; ++i)
        {
            Card card = cardList[i];
            int cardPoints = card.PointValue();
            if (card.suite == suiteToUse && suiteCardsPlayed < 3)
            {
                suitePoints += cardPoints;
                ++suiteCardsPlayed;
            }
            else if (cardPoints > notSuiteHighCard)
            {
                notSuiteHighCard = cardPoints;
                extraCard = card;
            }

            if (i < 3)
            {
                tripleCardScore += cardPoints;
            }
        }

        if (suiteCardsPlayed != 3 || tripleCardScore > suitePoints + notSuiteHighCard)
        {
            return cardList.Take(3);
        }
        else
        {
            return cardList.Where(card => card.suite == suiteToUse).Take(3).Concat(new Card[] { extraCard });
        }
    }

    public static int IdealScore(IEnumerable<Card> cards, IEnumerable<Card> playedCards)
    {
        return IdealHand(cards, playedCards).Sum(card => card.PointValue());
    }

    public static int ScoreHand(IEnumerable<Card> cards)
    {
        return cards.Sum(card => card.PointValue());
    }

    public static float ScoreOutcome(float winProbability, float foldProbability, float potSize, float bid)
    {
        return foldProbability * potSize + (1.0f - foldProbability) * (winProbability * (potSize + bid) - (1.0f - winProbability) * bid * 2.0f);
    }
}