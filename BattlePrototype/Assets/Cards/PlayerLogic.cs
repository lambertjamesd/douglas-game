using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public abstract class PlayerLogic {
    protected int index = 0;
    public PlayerHand hand = null;
    public int money = 1000;
    public Text moneyLabel;

    public int Index
    {
        get
        {
            return index;
        }
    }

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

    public void ExecuteTurn(shootout.TurnResult turn)
    {
        if (turn.chosenCard != null)
        {
            hand.PlayCard(turn.chosenCard);
            if (turn.fourthCard != null)
            {
                hand.PlayCard(turn.fourthCard);
            }
        }
    }

    public abstract IEnumerator StartTurn(List<List<Card>> onBoard, int currentBid, int currentBidScalar, int inPot, int currentTurn);
    public abstract shootout.TurnResult TurnResult();
}

public class HumanPlayerLogic : PlayerLogic
{
    private Button foldButton;
    private Transform guiTransform;
    private Text bidPreview;
    private RadioButtons multiplier;
    private Image multiplierShow;
    private Sprite[] multiplierSymbols;
    private IEnumerable<Button> cardSelection;
    private shootout.TurnResult choice = null;
    private int cardChoice = -1;
    private bool takingTurn = false;
    private Card chosenCard;
    private int minBid = 10;

    public HumanPlayerLogic(
        int index, 
        PlayerHand hand,
        Button foldButton,
        Transform guiTransform,
        Text bidPreview,
        RadioButtons multiplier,
        Image multiplierShow,
        Sprite[] multiplierSymbols,
        List<Button> cardSelection, 
        Text moneyLabel
    ) : base(index, hand, moneyLabel)  {
        this.foldButton = foldButton;
        this.guiTransform = guiTransform;
        this.bidPreview = bidPreview;
        this.multiplier = multiplier;
        this.multiplierShow = multiplierShow;
        this.multiplierSymbols = multiplierSymbols;
        this.cardSelection = cardSelection;

        this.multiplier.Change((multiplierIndex) =>
        {
            UpdateBid();
        });

        foldButton.onClick.AddListener(() =>
        {
            if (takingTurn)
            {
                choice = shootout.TurnResult.Fold();
            }
        });

        for (int i = 0; i < cardSelection.Count; ++i)
        {
            BindCardClick(cardSelection[i], i);
        }
        SetGUIVisible(false, -1, 1);
    }

    private void PositionGUI(int slotOffset = 0)
    {
        guiTransform.position = hand.boardSlots[hand.GetPlayedCards().Count + slotOffset].transform.position;
    }

    private void SetGUIVisible(bool value, int useBid, int bidMultiplier)
    {
        guiTransform.gameObject.SetActive(value);
        foldButton.gameObject.SetActive(value);

        PositionGUI(0);

        multiplier.gameObject.SetActive(useBid == -1);
        multiplierShow.gameObject.SetActive(useBid != -1);
        if (useBid != -1)
        {
            bidPreview.text = "$" + useBid.ToString();
        }
        else
        {
            multiplierShow.sprite = multiplierSymbols[bidMultiplier - 1];
        }
    }

    private void UpdateBid()
    {
        bidPreview.text = "$" + GetBid();
    }

    private int GetBid()
    {
        return Math.Min(money, minBid * (multiplier.Selected + 1));
    }

    private void BindCardClick(Button button, int index)
    {
        button.onClick.AddListener(() =>
        {
            Card pickedCard = index < hand.GetHand().Count ? hand.GetHand()[index] : null;

            if (takingTurn && pickedCard != null)
            {
                if (choice == null)
                {
                    choice = new shootout.TurnResult(GetBid(), pickedCard);
                    chosenCard = pickedCard;
                }
                else if (choice.chosenCard != pickedCard)
                {
                    choice.bid = GetBid();
                    choice.fourthCard = pickedCard;
                    chosenCard = pickedCard;
                }
            }
        });
    }

    public override IEnumerator StartTurn(List<List<Card>> onBoard, int currentBid, int currentBidScalar, int inPot, int currentTurn)
    {
        takingTurn = true;
        choice = null;
        cardChoice = -1;

        multiplier.SetSelected(0);
        UpdateBid();
        minBid = Math.Max(Math.Min((currentBid == -1) ? inPot / 2 : currentBid, money), 1);

        SetGUIVisible(true, currentBid, 1);

        while (choice == null || (CanPlayExtraCard(choice.chosenCard) && choice.fourthCard == null))
        {
            if (chosenCard != null)
            {
                PositionGUI(1);
                yield return hand.PutCardOnTable(chosenCard);
                chosenCard = null;
            }

            yield return null;
        }

        SetGUIVisible(false, -1, 1);

        if (chosenCard != null)
        {
            if (chosenCard != choice.fourthCard)
            {
                yield return hand.PutCardOnTable(chosenCard);
            }
            chosenCard = null;
        }

        takingTurn = false;
    }

    public override shootout.TurnResult TurnResult()
    {
        return choice;
    }
}

public class CardAIBase : PlayerLogic
{
    shootout.TurnResult choice = null;
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

    public virtual float PlayerFoldProbability(IEnumerable<Card> showingCards, IEnumerable<Card> oponentShowingCards, int turn, int bid, int inPot, bool isOpening)
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

    public int GetBid(int minBid, int points)
    {
        return minBid;
    }
    
    public virtual shootout.TurnResult ChooseCard(IEnumerable<Card> myShowing, IEnumerable<Card> theirShowing, int inPot, int currentBid, int currentBidScalar, int currentTurn)
    {
        if (myShowing.Count() == 0)
        {
            var pickedCardsList = PlayOrderCommon(IdealHand(hand.GetHand(), new Card[] { }));
            pickedCards = pickedCardsList.GetEnumerator();
            handScore = ScoreHand(pickedCardsList);
        }

        int minBid = inPot / 2;
        int bid = currentBid;
        bool shouldFold = false;
        bool isOpening = currentBid == -1;

        if (isOpening)
        {
            bid = GetBid(minBid, handScore);
            float expectedWinnings = ScoreOutcome(WinProbability(bid, handScore), PlayerFoldProbability(theirShowing, myShowing, currentTurn, minBid, inPot, isOpening), inPot, minBid);

            shouldFold = expectedWinnings < 0.0f;
        }
        else
        {
            shouldFold = ScoreOutcome(WinProbability(bid, handScore), PlayerFoldProbability(theirShowing, myShowing, currentTurn, currentBid, inPot, isOpening), inPot, bid) < 0.0f;
        }

        return shouldFold ? shootout.TurnResult.Fold() : new shootout.TurnResult(Math.Min(bid, money), PickCard());
    }

    public override IEnumerator StartTurn(List<List<Card>> onBoard, int currentBid, int currentBidScalar, int inPot, int currentTurn)
    {
        choice = ChooseCard(onBoard[index], onBoard[1 - index], inPot, currentBid, currentBidScalar, currentTurn);

        yield return hand.PutCardOnTable(choice.chosenCard);
        
        if (CanPlayExtraCard(choice.chosenCard) && choice.fourthCard == null)
        {
            choice.fourthCard = PickCard();
        }

        yield return null;
    }

    public override shootout.TurnResult TurnResult()
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

    public float ScoreOutcome(IEnumerable<Card> showingCards, IEnumerable<Card> oponentShowingCards, IEnumerable<Card> leftToPlay, float winProbability, int minBid, int moneyInPot, int turn, bool isOpening, out int bid)
    {
        bid = minBid;
        if (isOpening)
        {
            float score = float.MinValue;
            for (int i = 1; i <= 3; ++i)
            {
                float thisScore = ScoreOpeningOutcome(showingCards, oponentShowingCards, null, leftToPlay, false, winProbability, minBid, moneyInPot, turn);
                if (thisScore > score)
                {
                    score = thisScore;
                    bid = minBid * i;
                }
            }

            return score;
        }
        else
        {
            return ScoreResponseOutcome(showingCards, oponentShowingCards, null, leftToPlay, false, winProbability, minBid, moneyInPot, turn);
        }
    }

    public static float IncorporateFoldProbability(float foldProbability, int moneyInPot, int bid, float nextScore)
    {
        return foldProbability * moneyInPot - (1.0f - foldProbability) * (bid + nextScore);
    }

    public float ScoreOpeningOutcome(IEnumerable<Card> showingCards, IEnumerable<Card> oponentShowingCards, Card cardToPlay, IEnumerable<Card> leftToPlay, bool isOponent, float winProbability, int bid, int moneyInPot, int turn)
    {
        float next = float.MinValue;

        int nextMoneyInPot = moneyInPot + bid;
        int minBid = nextMoneyInPot / 2;

        if (isOponent)
        {
            float nextScore = ScoreOpeningOutcome(
                oponentShowingCards,
                showingCards,
                null,
                leftToPlay,
                false,
                1.0f - winProbability,
                minBid,
                nextMoneyInPot,
                turn + 1
            );

            next = IncorporateFoldProbability(PlayerFoldProbability(oponentShowingCards, showingCards, turn, minBid, nextMoneyInPot, false), moneyInPot, bid, nextScore);
        }
        else
        {
            foreach (Card toPlay in leftToPlay)
            {
                for (int multiplier = 1; multiplier <= 3; ++multiplier)
                {
                    float testOutcome = ScoreOpeningOutcome(
                        oponentShowingCards,
                        showingCards,
                        toPlay,
                        leftToPlay.Where((other) => other != toPlay),
                        true,
                        1.0f - winProbability,
                        minBid * multiplier,
                        nextMoneyInPot,
                        turn + 1
                       );

                    float foldProbability = PlayerFoldProbability(oponentShowingCards, showingCards, turn, minBid * multiplier, nextMoneyInPot, false);

                    next = Mathf.Max(IncorporateFoldProbability(foldProbability, moneyInPot, bid,  testOutcome), next);
                }
            }
        }


        return next;
    }

    public float ScoreResponseOutcome(IEnumerable<Card> showingCards, IEnumerable<Card> oponentShowingCards, Card cardPlayed, IEnumerable<Card> leftToPlay, bool isOponent, float winProbability, int bid, int moneyInPot, int turn)
    {
        if (turn == 2)
        {
            return moneyInPot * winProbability - (1.0f - winProbability) * bid;
        }
        else
        {
            float next = float.MinValue;

            int nextMoneyInPot = moneyInPot + bid;
            int minBid = nextMoneyInPot / 2;

            if (isOponent)
            {
                float nextScore = ScoreOpeningOutcome(
                    oponentShowingCards.Concat(new Card[] { cardPlayed }), 
                    showingCards, 
                    null, 
                    leftToPlay,
                    false, 
                    1.0f - winProbability, 
                    minBid, 
                    nextMoneyInPot, 
                    turn + 1
                );

                next = IncorporateFoldProbability(PlayerFoldProbability(oponentShowingCards, showingCards, turn + 1, minBid, nextMoneyInPot, true), moneyInPot, bid, nextScore);
            }
            else
            {
                foreach (Card toPlay in leftToPlay)
                {
                    float testOutcome = ScoreOpeningOutcome(
                        oponentShowingCards, 
                        showingCards.Concat(new Card[] { toPlay }), 
                        null, 
                        leftToPlay.Where((other) => other!= toPlay),
                        true, 
                        1.0f - winProbability, 
                        minBid,
                        nextMoneyInPot, 
                        turn + 1
                       );

                    float scorenext = IncorporateFoldProbability(PlayerFoldProbability(oponentShowingCards, showingCards, turn + 1, minBid, nextMoneyInPot, true), moneyInPot, bid, testOutcome);

                    if (scorenext > next)
                    {
                        next = scorenext;
                    }
                }
            }

            return next;
        }
    }
}