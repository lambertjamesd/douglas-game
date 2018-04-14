using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

namespace shootout
{
    public class TurnProbability
    {
        private LearningProbability[] winProbability;
        private LearningProbability[,] foldProbability;

        private int cardsPlayed;

        public TurnProbability(int remainingTurns)
        {
            winProbability = new LearningProbability[CardGameLogic.MAX_BID_SCALAR + 1];
            foldProbability = new LearningProbability[CardGameLogic.MAX_BID_SCALAR + 1, remainingTurns];
            cardsPlayed = 3 - remainingTurns;

            for (int bid = 0; bid <= CardGameLogic.MAX_BID_SCALAR; ++bid)
            {
                winProbability[bid] = new LearningProbability(CardProbability.MaxScore, cardsPlayed * Card.MAX_CARD_SCORE);
                for (int turn = 0; turn < remainingTurns; ++turn)
                {
                    foldProbability[bid, turn] = new LearningProbability((turn + cardsPlayed) * Card.MAX_CARD_SCORE, cardsPlayed * Card.MAX_CARD_SCORE);
                }
            }
        }

        public void GenerateProbibility(int sampleCount, bool myMatch, bool theirsMatch)
        {
            int remainingTurns = 3 - cardsPlayed;

            for (int myScore = CardProbability.MinScore; myScore <= CardProbability.MaxScore; ++myScore)
            {
                for (int theirScore = cardsPlayed * Card.MIN_CARD_SCORE; theirScore <= cardsPlayed * Card.MAX_CARD_SCORE; ++theirScore)
                {
                    winProbability[0].SetProbability(myScore, theirScore, CardProbability.CalculateProbabilityOfWin(myScore, theirScore, cardsPlayed, myMatch, sampleCount));
                }
            }

            for (int turn = 0; turn < remainingTurns; ++turn)
            {
                int myCardsPlayed = turn + cardsPlayed;
                for (int myScore = myCardsPlayed * Card.MIN_CARD_SCORE; myScore <= myCardsPlayed * Card.MAX_CARD_SCORE; ++myScore)
                {
                    for (int theirScore = cardsPlayed * Card.MIN_CARD_SCORE; theirScore <= cardsPlayed * Card.MAX_CARD_SCORE; ++theirScore)
                    {
                        foldProbability[0, turn].SetProbability(myScore, theirScore, CardProbability.CalculateProbabilityOfFold(myCardsPlayed, myScore, myMatch, cardsPlayed, theirScore, theirsMatch, sampleCount));
                     }
                }
            }

            for (int bid = 1; bid <= CardGameLogic.MAX_BID_SCALAR; ++bid)
            {
                winProbability[bid].CopyFrom(winProbability[0]);
                for (int turn = 0; turn < remainingTurns; ++turn)
                {
                    foldProbability[bid, turn].CopyFrom(foldProbability[0, turn]);
                }
            }
        }

        public void ModifyByBid()
        {
            int remainingTurns = 3 - cardsPlayed;


            for (int bid = 2; bid <= CardGameLogic.MAX_BID_SCALAR; ++bid)
            {

                for (int myScore = CardProbability.MinScore; myScore <= CardProbability.MaxScore; ++myScore)
                {
                    for (int theirScore = cardsPlayed * Card.MIN_CARD_SCORE; theirScore <= cardsPlayed * Card.MAX_CARD_SCORE; ++theirScore)
                    {
                        float exitingProbability = winProbability[bid].GetProbability(myScore, theirScore);
                        winProbability[bid].SetProbability(myScore, theirScore, exitingProbability / bid);
                    }
                }

                for (int turn = 0; turn < remainingTurns; ++turn)
                {
                    int myCardsPlayed = turn + cardsPlayed;
                    for (int myScore = myCardsPlayed * Card.MIN_CARD_SCORE; myScore <= myCardsPlayed * Card.MAX_CARD_SCORE; ++myScore)
                    {
                        for (int theirScore = cardsPlayed * Card.MIN_CARD_SCORE; theirScore <= cardsPlayed * Card.MAX_CARD_SCORE; ++theirScore)
                        {
                            float exitingProbability = foldProbability[bid, turn].GetProbability(myScore, theirScore);
                            foldProbability[bid, turn].SetProbability(myScore, theirScore, CardProbability.MakeMoreLikely(exitingProbability, bid));
                        }
                    }
                }
            }
        }

        /**
         * if theirBidScalar is 0 then this player goes first
         */
        public float GetWinProbability(int myMaxScore, int theirBidScalar, int theirShowingScore)
        {
            return winProbability[theirBidScalar].GetProbability(myMaxScore, theirShowingScore);
        }

        /**
         * if theirBidScalar is 0 then the other player goes first
         */
        public float GetFoldProbability(int myCardsPlayed, int myScoreShowing, int myBidScalar, int theirShowingScore)
        {
            return foldProbability[myBidScalar, myCardsPlayed - cardsPlayed].GetProbability(myScoreShowing, theirShowingScore);
        }

        public void Write(BinaryWriter output)
        {
            for (int i = 0; i < winProbability.Length; ++i)
            {
                winProbability[i].Write(output);
            }

            for (int i = 0; i < foldProbability.GetLength(0); ++i)
            {
                for (int j = 0; j < foldProbability.GetLength(1); ++j)
                {
                    foldProbability[i, j].Write(output);
                }
            }
        }

        public void Read(BinaryReader input)
        {
            for (int i = 0; i < winProbability.Length; ++i)
            {
                winProbability[i].Read(input);
            }

            for (int i = 0; i < foldProbability.GetLength(0); ++i)
            {
                for (int j = 0; j < foldProbability.GetLength(1); ++j)
                {
                    foldProbability[i, j].Read(input);
                }
            }
        }
    }
    
    public class AllTurnProbabilities
    {
        private TurnProbability[] turns = new TurnProbability[]
        {
            new TurnProbability(3),
            new TurnProbability(2),
        };

        private TurnProbability[,] turnThreeProbability = new TurnProbability[2, 2]
        {
            {new TurnProbability(1), new TurnProbability(1)},
            {new TurnProbability(1), new TurnProbability(1)},
        };
        
        /**
         * if theirBidScalar is 0 then this player goes first
         */
        public float GetWinProbability(int currentTurn, int myMaxScore, int theirBidScalar, int theirShowingScore, bool theyShowingMatch)
        {
            if (currentTurn < 2)
            {
                return turns[currentTurn].GetWinProbability(myMaxScore, theirBidScalar, theirShowingScore);
            }
            else
            {
                return turnThreeProbability[0, theyShowingMatch ? 1 : 0].GetWinProbability(myMaxScore, theirBidScalar, theirShowingScore);
            }
        }

        /**
         * if theirBidScalar is 0 then the other player goes first
         */
        public float GetFoldProbability(int currentTurn, int cardsPlayed, int myScoreShowing, bool meShowingMatch, int myBidScalar, int theirShowingScore, bool theyShowingMatch)
        {
            if (currentTurn < 2)
            {
                return turns[currentTurn].GetFoldProbability(cardsPlayed, myScoreShowing, myBidScalar, theirShowingScore);
            }
            else
            {
                return turnThreeProbability[meShowingMatch ? 1 : 0, theyShowingMatch ? 1 : 0].GetFoldProbability(cardsPlayed, myScoreShowing, myBidScalar, theirShowingScore);
            }
        }


        public void GenerateProbibility(int sampleCount)
        {
            turns[0].GenerateProbibility(sampleCount, true, true);
            turns[1].GenerateProbibility(sampleCount, true, true);
            turnThreeProbability[0, 0].GenerateProbibility(sampleCount, false, false);
            turnThreeProbability[0, 1].GenerateProbibility(sampleCount, false, true);
            turnThreeProbability[1, 0].GenerateProbibility(sampleCount, true, false);
            turnThreeProbability[1, 1].GenerateProbibility(sampleCount, true, true);
        }

        public void Write(BinaryWriter output)
        {
            turns[0].Write(output);
            turns[1].Write(output);
            turnThreeProbability[0, 0].Write(output);
            turnThreeProbability[0, 1].Write(output);
            turnThreeProbability[1, 0].Write(output);
            turnThreeProbability[1, 1].Write(output);
        }

        public void Read(BinaryReader input)
        {
            turns[0].Read(input);
            turns[1].Read(input);
            turnThreeProbability[0, 0].Read(input);
            turnThreeProbability[0, 1].Read(input);
            turnThreeProbability[1, 0].Read(input);
            turnThreeProbability[1, 1].Read(input);
        }

        public void ModifyByBid()
        {
            turns[0].ModifyByBid();
            turns[1].ModifyByBid();
            turnThreeProbability[0, 0].ModifyByBid();
            turnThreeProbability[0, 1].ModifyByBid();
            turnThreeProbability[1, 0].ModifyByBid();
            turnThreeProbability[1, 1].ModifyByBid();
        }
    }
}

