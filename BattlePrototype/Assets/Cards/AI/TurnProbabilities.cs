using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
}

