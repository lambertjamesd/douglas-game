using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace shootout
{
    public class AIPlayerState
    {
        public Card[] hand = new Card[5];
        public Card[] visibleCards = new Card[4];
        public Card pendingCard = null;
        public Card fourthCard = null;
        public int cardsPlayed = 0;
        public int visibleScore = 0;
        public int initialVisibleScore = 0;
        public bool allMatch = true;
        public bool initialAllMatch = true;

        public AIPlayerState()
        {

        }

        public AIPlayerState(IEnumerable<Card> myHand, IEnumerable<Card> playedCards)
        {
            int i = 0;
            foreach (Card card in myHand)
            {
                hand[i] = card;
                ++i;
            }
            i = 0;
            foreach (Card card in playedCards)
            {
                visibleCards[i] = card;
                ++i;
            }
            cardsPlayed = i;
            visibleScore = playedCards.Sum(item => item.PointValue());
            initialVisibleScore = visibleScore;
            allMatch = cardsPlayed == 2 ? visibleCards[0].suite == visibleCards[1].suite : true;
            initialAllMatch = allMatch;
        }

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
            initialVisibleScore = source.initialVisibleScore;
            allMatch = source.allMatch;
            initialAllMatch = source.initialAllMatch;
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

            for (int i = 1; i < cardsPlayed && allMatch; ++i)
            {
                if (visibleCards[i].suite != visibleCards[0].suite)
                {
                    allMatch = false;
                }
            }

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
        public bool allMatch = true;

        public OtherPlayerState()
        {

        }

        public OtherPlayerState(IEnumerable<Card> showing)
        {
            int i = 0;
            foreach (Card card in showing)
            {
                visibleCards[i] = card;
                ++i;
            }
            cardsPlayed = i;
            visibleScore = showing.Sum(item => item.PointValue());
            allMatch = cardsPlayed == 2 ? visibleCards[0].suite == visibleCards[1].suite : true;
        }

        public void CloneFrom(OtherPlayerState source)
        {
            for (int i = 0; i < visibleCards.Length; ++i)
            {
                visibleCards[i] = source.visibleCards[i];
            }
            cardsPlayed = source.cardsPlayed;
            visibleScore = source.visibleScore;
            allMatch = source.allMatch;
        }
    }

    public class TurnResult
    {
        public float value;
        public Card chosenCard;
        public int bid;
        public int bidScalar;
        public Card fourthCard;

        public TurnResult(int bid, int bidScalar, Card chosenCard)
        {
            this.bid = bid;
            this.bidScalar = bidScalar;
            this.chosenCard = chosenCard;
        }

        public bool IsFold()
        {
            return chosenCard == null;
        }

        public static TurnResult Fold()
        {
            return new TurnResult(0, 0, null);
        }

        public int PointValue()
        {
            return (chosenCard == null ? 0 : chosenCard.PointValue()) + (fourthCard == null ? 0 : fourthCard.PointValue());
        }
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
        public int currentBidScalar = 0;

        public static int checkCount = 0;

        public CardGameState()
        {

        }

        public CardGameState(IEnumerable<Card> hand, IEnumerable<Card> minePlayed, IEnumerable<Card> theirsPlayed, bool isMeFirst, bool isFirst, int pot, int bid, int bidScalar)
        {
            aiPlayer = new AIPlayerState(hand, minePlayed);
            otherPlayer = new OtherPlayerState(theirsPlayed);
            round = otherPlayer.cardsPlayed;
            isFirstTurn = isFirst;
            isAIFirst = isMeFirst;
            amountInPot = pot;
            currentBid = bid;
            currentBidScalar = bidScalar;
        }

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
            result.currentBidScalar = currentBidScalar;

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

        private float FinishAITurn(int bid, int bidScalar, CardGameAI ai)
        {
            currentBid = bid;
            currentBidScalar = bidScalar;
            amountInPot += currentBid;
            AdvanceTurn();

            TurnResult subResult = CaclulateOptimalTurn(ai);

            return subResult.value - bid;
        }

        private TurnResult TakeAITurn(int bid, int bidScalar, CardGameAI ai)
        {
            TurnResult result = shootout.TurnResult.Fold();
        

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

                        float score = fourthPlayState.FinishAITurn(bid, bidScalar, ai);

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
                    float score = nextState.FinishAITurn(bid, bidScalar, ai);

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

        private float TakeOtherPlayerTurn(float foldProb, int bid, int bidScalar, CardGameAI ai)
        {
            // other player responds
            CardGameState nextState = Clone();
            nextState.currentBid = bid;
            nextState.currentBidScalar = bidScalar;
            nextState.amountInPot += bid;
            nextState.AdvanceTurn();

            TurnResult subResult = nextState.CaclulateOptimalTurn(ai);

            return subResult.value * (1 - foldProb) + amountInPot * foldProb;
        }

        public TurnResult CaclulateOptimalTurn(CardGameAI ai)
        {
            TurnResult result = TurnResult.Fold();

            if (round == 3)
            {
                ++checkCount;
                result.value = ai.winProbability(aiPlayer, otherPlayer, isAIFirst, currentBidScalar) * amountInPot;
            }
            else
            {
                int minBid = (amountInPot - currentBid) / 2;
                int cardSlot = aiPlayer.cardsPlayed;

                if (isFirstTurn && isAIFirst)
                {
                    // this player goes first
                    for (int bidScalar = 1; bidScalar <= 3; ++bidScalar)
                    {
                        TurnResult subResult = TakeAITurn(bidScalar * minBid, bidScalar, ai);

                        if (subResult.value > result.value)
                        {
                            result = subResult;
                        }
                    }
                }
                else if (!isFirstTurn && isAIFirst)
                {
                    // other player responds
                    float foldProb = ai.foldProbaility(aiPlayer, otherPlayer, isAIFirst, currentBidScalar);
                    result.value = TakeOtherPlayerTurn(foldProb, currentBid, currentBidScalar, ai);
                }
                else if (!isFirstTurn && !isAIFirst)
                {
                    // this player responds
                    result = TakeAITurn(currentBid, currentBidScalar, ai);
                }
                else
                {
                    // other player goes first
                    float foldProb = ai.foldProbaility(aiPlayer, otherPlayer, isAIFirst, currentBidScalar);

                    float scoreResult = float.PositiveInfinity;

                    for (int bidScalar = 1; bidScalar <= 3; ++bidScalar)
                    {
                        float singleResult = TakeOtherPlayerTurn(foldProb, bidScalar * minBid, bidScalar, ai);

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
        public delegate float WinProbability(AIPlayerState myState, OtherPlayerState otherState, bool isFirst, int bidScalar);
        public delegate float FoldProbability(AIPlayerState myState, OtherPlayerState otherState, bool isFirst, int bidScalar);
        public delegate void Learn(List<TurnResult> turnResults, bool amIFirst, int myTopScore, int theirTopScore, bool myDouble, bool theirDouble);

        public WinProbability winProbability;
        public FoldProbability foldProbaility;
        public Learn learn;

        public static CardGameAI DumbAI()
        {
            CardGameAI result = new CardGameAI();
            result.winProbability = (me, them, isFirst, bidScalar) => 1;
            result.foldProbaility = (me, them, isFirst, bidScalar) => 0;
            return result;
        }

        public static CardGameAI CalculatedAI()
        {
            CardGameAI result = new CardGameAI();
            result.winProbability = (me, them, isFirst, bidScalar) => CardProbability.winProbability[me.visibleScore];
            result.foldProbaility = (me, them, isFirst, bidScalar) => 0;
            return result;
        }

        public static CardGameAI TurnProbability(AllTurnProbabilities probability)
        {
            CardGameAI result = new CardGameAI();
            result.winProbability = (me, them, isFirst, bidScalar) => probability.GetWinProbability(them.cardsPlayed, me.visibleScore, isFirst ? 0 : bidScalar, them.visibleScore, them.allMatch);
            result.foldProbaility = (me, them, isFirst, bidScalar) => probability.GetFoldProbability(them.cardsPlayed, me.cardsPlayed, me.visibleScore, me.initialAllMatch, isFirst ? bidScalar : 0, them.visibleScore, them.allMatch);
            result.learn = (turnResults, amIFirst, myTopScore, theirTopScore, myDouble, theirDouble) => probability.Learn(turnResults, amIFirst, myTopScore, theirTopScore, myDouble, theirDouble);
            return result;
        }
    }
}