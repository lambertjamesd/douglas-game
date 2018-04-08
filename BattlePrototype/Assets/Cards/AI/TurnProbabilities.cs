using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

namespace shootout
{
    public class SingleTurnProbabilities
    {
        private LearningProbability singleCardOpenWinProbability;
        private LearningProbability[] singeCardResponseWinProbability;
        private LearningProbability singleCardOpenFoldProbability;
        private LearningProbability[] singleCardResponseFoldProbability;

        public SingleTurnProbabilities(int cardsPlayed)
        {
            singleCardOpenWinProbability = new LearningProbability(CardProbability.MaxScore, cardsPlayed * Card.MAX_CARD_SCORE);
            singeCardResponseWinProbability = new LearningProbability[]
            {
                new LearningProbability(CardProbability.MaxScore, cardsPlayed * Card.MAX_CARD_SCORE),
                new LearningProbability(CardProbability.MaxScore, cardsPlayed * Card.MAX_CARD_SCORE),
                new LearningProbability(CardProbability.MaxScore, cardsPlayed * Card.MAX_CARD_SCORE),
            };
            singleCardOpenFoldProbability = new LearningProbability(cardsPlayed * Card.MAX_CARD_SCORE, cardsPlayed * Card.MAX_CARD_SCORE);
            singleCardResponseFoldProbability = new LearningProbability[] {
                new LearningProbability(cardsPlayed * Card.MAX_CARD_SCORE, cardsPlayed * Card.MAX_CARD_SCORE),
                new LearningProbability(cardsPlayed * Card.MAX_CARD_SCORE, cardsPlayed * Card.MAX_CARD_SCORE),
                new LearningProbability(cardsPlayed * Card.MAX_CARD_SCORE, cardsPlayed * Card.MAX_CARD_SCORE),
            };
        }

        public float GetFoldProbability(int myShowing, int theirShowing, bool meFirst, int bidScalar)
        {
            if (meFirst)
            {
                return singleCardResponseFoldProbability[bidScalar - 1].GetProbability(myShowing, theirShowing);
            }
            else
            {
                return singleCardOpenFoldProbability.GetProbability(myShowing, theirShowing);
            }
        }

        public float GetWinProbability(int myScore, int theirShowing, bool meFirst, int bidScalar)
        {
            if (meFirst)
            {
                return singleCardOpenWinProbability.GetProbability(myScore, theirShowing);
            }
            else
            {
                return singeCardResponseWinProbability[bidScalar - 1].GetProbability(myScore, theirShowing);
            }
        }

        private void SetWinProbability(int myScore, int theirScore, float value)
        {
            singleCardOpenWinProbability.SetProbability(myScore, theirScore, value);
            foreach (LearningProbability prob in singeCardResponseWinProbability) {
                prob.SetProbability(myScore, theirScore, value);
            }
        }

        private void SetFoldProbability(int myScore, int theirScore, float value)
        {
            singleCardOpenFoldProbability.SetProbability(myScore, theirScore, value);
            foreach (LearningProbability prob in singleCardResponseFoldProbability)
            {
                prob.SetProbability(myScore, theirScore, value);
            }
        }

        public void CopyFrom(SingleTurnProbabilities other)
        {
            singleCardOpenWinProbability.CopyFrom(other.singleCardOpenWinProbability);
            for (int i = 0; i < singeCardResponseWinProbability.Length; ++i)
            {
                singeCardResponseWinProbability[i].CopyFrom(other.singeCardResponseWinProbability[i]);
            }
            singleCardOpenFoldProbability.CopyFrom(other.singleCardOpenFoldProbability);
            for (int i = 0; i < singleCardResponseFoldProbability.Length; ++i)
            {
                singleCardResponseFoldProbability[i].CopyFrom(other.singleCardResponseFoldProbability[i]);
            }
        }

        public void Write(BinaryWriter output)
        {
            singleCardOpenWinProbability.Write(output);
            for (int i = 0; i < singeCardResponseWinProbability.Length; ++i)
            {
                singeCardResponseWinProbability[i].Write(output);
            }
            singleCardOpenFoldProbability.Write(output);
            for (int i = 0; i < singleCardResponseFoldProbability.Length; ++i)
            {
                singleCardResponseFoldProbability[i].Write(output);
            }
        }

        public void Read(BinaryReader input)
        {
            singleCardOpenWinProbability.Read(input);
            for (int i = 0; i < singeCardResponseWinProbability.Length; ++i)
            {
                singeCardResponseWinProbability[i].Read(input);
            }
            singleCardOpenFoldProbability.Read(input);
            for (int i = 0; i < singleCardResponseFoldProbability.Length; ++i)
            {
                singleCardResponseFoldProbability[i].Read(input);
            }
        }

        public void InitProbability(int cardsPlayed, bool myMatches, bool theirMatches)
        {
            for (int myScore = CardProbability.MinScore; myScore <= CardProbability.MaxScore; ++myScore)
            {
                for (int theirScore = Card.MIN_CARD_SCORE * cardsPlayed; theirScore < Card.MAX_CARD_SCORE * cardsPlayed; ++theirScore)
                {
                    SetWinProbability(myScore, theirScore, CardProbability.CalculateProbabilityOfWin(myScore, theirScore, cardsPlayed, theirMatches));
                }
            }

            for (int myShowing = Card.MIN_CARD_SCORE * cardsPlayed; myShowing < Card.MAX_CARD_SCORE * cardsPlayed; ++myShowing)
            {
                for (int theirShowing = Card.MIN_CARD_SCORE * cardsPlayed; theirShowing < Card.MAX_CARD_SCORE * cardsPlayed; ++theirShowing)
                {
                    SetFoldProbability(myShowing, theirShowing, CardProbability.CalculateProbabilityOfFold(cardsPlayed, myShowing, myMatches, theirShowing, theirMatches));
                }
            }
        }
    }

    public class TurnProbabilities
    {
        private float[] firstTurnFoldProbability = new float[] {
            0.5f,
            0.5f,
            0.5f,
        };

        private SingleTurnProbabilities firstTurn = new SingleTurnProbabilities(1);
        private SingleTurnProbabilities[,] secondTurn = new SingleTurnProbabilities[,]
        {
            {new SingleTurnProbabilities(2), new SingleTurnProbabilities(2)},
            {new SingleTurnProbabilities(2), new SingleTurnProbabilities(2)},
        };

        public void Write(BinaryWriter output)
        {
            output.Write(firstTurnFoldProbability[0]);
            output.Write(firstTurnFoldProbability[1]);
            output.Write(firstTurnFoldProbability[2]);

            firstTurn.Write(output);
            secondTurn[0, 0].Write(output);
            secondTurn[0, 1].Write(output);
            secondTurn[1, 0].Write(output);
            secondTurn[1, 1].Write(output);
        }

        public void Read(BinaryReader input)
        {
            firstTurnFoldProbability[0] = input.ReadSingle();
            firstTurnFoldProbability[1] = input.ReadSingle();
            firstTurnFoldProbability[2] = input.ReadSingle();

            firstTurn.Read(input);
            secondTurn[0, 0].Read(input);
            secondTurn[0, 1].Read(input);
            secondTurn[1, 0].Read(input);
            secondTurn[1, 1].Read(input);
        }

        public void InitProbability()
        {
            firstTurn.InitProbability(1, true, true);
            secondTurn[0, 0].InitProbability(2, false, false);
            secondTurn[0, 1].InitProbability(2, false, true);
            secondTurn[1, 0].InitProbability(2, true, false);
            secondTurn[1, 1].InitProbability(2, true, true);
        }

        public float GetWinProbability(int cardsPlayed, int myScore, bool myShowingMatch, int theirShowingScore, bool theirShowingMatch, bool meFirst, int bidScalar)
        {
            if (cardsPlayed == 0)
            {
                return CardProbability.winProbability[myScore];
            }
            else if (cardsPlayed == 1)
            {
                return firstTurn.GetWinProbability(myScore, theirShowingScore, meFirst, bidScalar);
            }
            else if (cardsPlayed == 2)
            {
                return secondTurn[myShowingMatch ? 1 : 0, theirShowingMatch ? 1 : 0].GetWinProbability(myScore, theirShowingScore, meFirst, bidScalar);
            }
            else
            {
                throw new System.Exception("Can't handle this number of cards");
            }
        }

        public float GetFoldProbability(int cardsPlayed, int myShowingScore, bool myShowingMatch, int theirShowingScore, bool theirShowingMatch, bool meFirst, int bidScalar)
        {
            if (cardsPlayed == 0)
            {
                if (meFirst)
                {
                    return firstTurnFoldProbability[bidScalar - 1];
                }
                else
                {
                    return 0.5f;
                }
            }
            else if (cardsPlayed == 1)
            {
                return firstTurn.GetFoldProbability(myShowingScore, theirShowingScore, meFirst, bidScalar);
            }
            else if (cardsPlayed == 2)
            {
                return secondTurn[myShowingMatch ? 1 : 0, theirShowingMatch ? 1 : 0].GetFoldProbability(myShowingScore, theirShowingScore, meFirst, bidScalar);
            }
            else
            {
                throw new System.Exception("Can't handle this number of cards");
            }
        }
    }

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
                    foldProbability[bid, turn] = new LearningProbability(cardsPlayed * Card.MAX_CARD_SCORE, cardsPlayed * Card.MAX_CARD_SCORE);
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
        public float GetFoldProbability(int cardsPlayed, int myScoreShowing, int myBidScalar, int theirShowingScore)
        {
            return foldProbability[myBidScalar, cardsPlayed - cardsPlayed].GetProbability(myScoreShowing, theirShowingScore);
        }

        public void FromOldStyle(TurnProbabilities old, bool meShowingMatch, bool themShowingMatch)
        {
            for (int bid = 0; bid <= CardGameLogic.MAX_BID_SCALAR; ++bid)
            {
                winProbability[bid] = new LearningProbability(CardProbability.MaxScore, cardsPlayed * Card.MAX_CARD_SCORE);
                for (int myScore = 0; myScore <= CardProbability.MaxScore; ++myScore)
                {
                    for (int theirScore = 0; theirScore <= cardsPlayed * Card.MAX_CARD_SCORE; ++theirScore)
                    {
                        winProbability[bid].SetProbability(myScore, theirScore, old.GetWinProbability(cardsPlayed, myScore, false, theirScore, themShowingMatch, bid == 0, bid));
                    }
                }

                int remainingTurns = 3 - cardsPlayed;

                for (int turn = 0; turn < remainingTurns; ++turn)
                {
                    for (int myScore = 0; myScore <= cardsPlayed * Card.MAX_CARD_SCORE; ++myScore)
                    {
                        for (int theirScore = 0; theirScore <= cardsPlayed * Card.MAX_CARD_SCORE; ++theirScore)
                        {
                            foldProbability[bid, turn].SetProbability(myScore, theirScore, old.GetFoldProbability(cardsPlayed, myScore, meShowingMatch, theirScore, themShowingMatch, bid != 0, bid));
                        }
                    }
                }
            }
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

        public void FromOldStyle(TurnProbabilities old)
        {
            turns[0].FromOldStyle(old, true, true);
            turns[1].FromOldStyle(old, true, true);
            turnThreeProbability[0, 0].FromOldStyle(old, false, false);
            turnThreeProbability[0, 1].FromOldStyle(old, false, true);
            turnThreeProbability[1, 0].FromOldStyle(old, true, false);
            turnThreeProbability[1, 1].FromOldStyle(old, true, true);
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
    }
}

