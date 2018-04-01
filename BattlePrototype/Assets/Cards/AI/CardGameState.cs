using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AIPlayerState
{
    public Card[] hand = new Card[5];
    public Card[] visibleCards = new Card[4];
    public Card pendingCard = null;
    public Card fourthCard = null;
    public int cardsPlayed = 0;
    public int visibleScore = 0;

    public delegate void CardCallback(int index);

    public void ForEachInHand(CardCallback callback)
    {
        for (int i = 0; i < hand.Length; ++i)
        {
            if (hand[i] != null)
            {
                callback(i);
            }
        }
    }

    public void CloneFrom(AIPlayerState source)
    {
        for (int i = 0; i < hand.Length; ++i)
        {
            hand[i] = source.hand[i];
        }

        for (int i = 0; i < visibleCards.Length; ++i)
        {
            visibleCards[i] = source.visibleCards[i];
        }

        pendingCard = source.pendingCard;
        cardsPlayed = source.cardsPlayed;
        visibleScore = source.visibleScore;
    }

    public bool CanPlayTriple()
    {
        if (visibleCards[0] != null)
        {
            for (int i = 1; i < visibleCards.Length && visibleCards[i] != null; ++i)
            {
                if (visibleCards[0].suite != visibleCards[i].suite)
                {
                    return false;
                }
            }

            if (pendingCard != null && pendingCard.suite != visibleCards[0].suite)
            {
                return false;
            }
        }

        return true;
    }

    public Card PlayCard(int index)
    {
        pendingCard = hand[index];
        hand[index] = null;
        return pendingCard;
    }

    public Card PlayFourthCard(int index)
    {
        fourthCard = hand[index];
        hand[index] = null;
        return fourthCard;
    }

    public void RevealCard()
    {
        visibleCards[cardsPlayed] = pendingCard;
        visibleScore += pendingCard.PointValue();
        pendingCard = null;
        ++cardsPlayed;

        if (fourthCard != null)
        {
            visibleCards[cardsPlayed] = fourthCard;
            visibleScore += fourthCard.PointValue();
            fourthCard = null;
            ++cardsPlayed;
        }
    }
}

public class OtherPlayerState
{
    public Card[] visibleCards = new Card[4];
    public int cardsPlayed = 0;
    public int visibleScore = 0;
    public bool canTriple = false;

    public void CloneFrom(OtherPlayerState source)
    {
        for (int i = 0; i < visibleCards.Length; ++i)
        {
            visibleCards[i] = source.visibleCards[i];
        }

        visibleScore = source.visibleScore;
        canTriple = source.canTriple;
    }
}

public class TurnResult
{
    public float value;
    public Card chosenCard;
    public int bid;
    public Card fourthCard;
}

public class CardGameState
{
    public AIPlayerState aiPlayer = new AIPlayerState();
    public OtherPlayerState otherPlayer = new OtherPlayerState();

    public int round = 0;
    public bool isFirstTurn = true;
    public bool isAIFirst = true;
    public int amountInPot = 0;
    public int currentBid = 0;

    public static int checkCount = 0;

    public CardGameState Clone()
    {
        CardGameState result = new CardGameState();

        result.aiPlayer.CloneFrom(aiPlayer);
        result.otherPlayer.CloneFrom(otherPlayer);
        result.round = round;
        result.isFirstTurn = isFirstTurn;
        result.isAIFirst = isAIFirst;
        result.amountInPot = amountInPot;
        result.currentBid = currentBid;

        return result;
    }

    public void AdvanceTurn()
    {
        if (isFirstTurn)
        {
            isFirstTurn = false;
        }
        else
        {
            ++round;
            aiPlayer.RevealCard();
            isAIFirst = !isAIFirst;
            isFirstTurn = true;
        }
    }

    private float FinishAITurn(int bid, CardGameAI ai)
    {
        currentBid = bid;
        amountInPot += currentBid;
        AdvanceTurn();

        TurnResult subResult = CaclulateOptimalTurn(ai);

        return subResult.value - bid;
    }

    private TurnResult TakeAITurn(int bid, CardGameAI ai)
    {
        TurnResult result = new TurnResult();
        

        aiPlayer.ForEachInHand((index) =>
        {
            CardGameState nextState = Clone();

            Card cardPlayed = nextState.aiPlayer.PlayCard(index);

            if (round == 2 && nextState.aiPlayer.CanPlayTriple())
            {
                nextState.aiPlayer.ForEachInHand((fourthIndex) =>
                {
                    CardGameState fourthPlayState = nextState.Clone();

                    Card fourthCard = fourthPlayState.aiPlayer.PlayFourthCard(fourthIndex);

                    float score = fourthPlayState.FinishAITurn(bid, ai);

                    if (score > result.value)
                    {
                        result.value = score;
                        result.chosenCard = cardPlayed;
                        result.bid = nextState.currentBid;
                        result.fourthCard = fourthCard;
                    }
                });
            }
            else
            {
                float score = nextState.FinishAITurn(bid, ai);

                if (score > result.value)
                {
                    result.value = score;
                    result.chosenCard = cardPlayed;
                    result.bid = nextState.currentBid;
                }
            }
        });

        return result;
    }

    private float TakeOtherPlayerTurn(float foldProb, int bid, CardGameAI ai)
    {
        // other player responds
        CardGameState nextState = Clone();
        nextState.amountInPot += bid;
        nextState.AdvanceTurn();

        TurnResult subResult = nextState.CaclulateOptimalTurn(ai);

        return subResult.value * (1 - foldProb) + amountInPot * foldProb;
    }

    public TurnResult CaclulateOptimalTurn(CardGameAI ai)
    {
        TurnResult result = new TurnResult();

        if (round == 3)
        {
            ++checkCount;
            result.value = ai.winProbability(aiPlayer, otherPlayer) * amountInPot;
        }
        else
        {
            int minBid = amountInPot / 2;
            int cardSlot = aiPlayer.cardsPlayed;

            if (isFirstTurn && isAIFirst)
            {
                // this player goes first
                for (int bidScalar = 1; bidScalar <= 3; ++bidScalar)
                {
                    TurnResult subResult = TakeAITurn(bidScalar * minBid, ai);

                    if (subResult.value > result.value)
                    {
                        result = subResult;
                    }
                }
            }
            else if (!isFirstTurn && isAIFirst)
            {
                // other player responds
                float foldProb = ai.foldProbaility(aiPlayer, otherPlayer);
                result.value = TakeOtherPlayerTurn(foldProb, currentBid, ai);
            }
            else if (!isFirstTurn && !isAIFirst)
            {
                // this player responds
                result = TakeAITurn(currentBid, ai);
            }
            else
            {
                // other player goes first
                float foldProb = ai.foldProbaility(aiPlayer, otherPlayer);

                float scoreResult = float.PositiveInfinity;

                for (int bidScalar = 1; bidScalar <= 3; ++bidScalar)
                {
                    float singleResult = TakeOtherPlayerTurn(foldProb, bidScalar * minBid, ai);

                    if (singleResult < scoreResult)
                    {
                        scoreResult = singleResult;
                    }
                }

                result.value = scoreResult;
            }
        }

        return result;
    }
}

public class CardGameAI
{
    public delegate float WinProbability(AIPlayerState myState, OtherPlayerState otherState);
    public delegate float FoldProbability(AIPlayerState myState, OtherPlayerState otherState);

    public WinProbability winProbability;
    public FoldProbability foldProbaility;

    public static CardGameAI DumbAI()
    {
        CardGameAI result = new CardGameAI();
        result.winProbability = (me, them) => 1;
        result.foldProbaility = (me, them) => 0;
        return result;
    }

    public static CardGameAI CalculatedAI()
    {
        CardGameAI result = new CardGameAI();
        result.winProbability = (me, them) => CardProbability.winProbability[me.visibleScore];
        result.foldProbaility = (me, them) => 0;
        return result;
    }
}