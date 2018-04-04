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

        public SingleTurnProbabilities(int maxScore)
        {
            singleCardOpenWinProbability = new LearningProbability(maxScore);
            singeCardResponseWinProbability = new LearningProbability[]
            {
                new LearningProbability(maxScore),
                new LearningProbability(maxScore),
                new LearningProbability(maxScore),
            };
            singleCardOpenFoldProbability = new LearningProbability(maxScore);
            singleCardResponseFoldProbability = new LearningProbability[] {
                new LearningProbability(maxScore),
                new LearningProbability(maxScore),
                new LearningProbability(maxScore),
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
    }

    public class TurnProbabilities
    {
        private float[] firstTurnFoldProbability = new float[] {
            0.5f,
            0.5f,
            0.5f,
        };

        private SingleTurnProbabilities firstTurn = new SingleTurnProbabilities(Card.MAX_CARD_SCORE);
        private SingleTurnProbabilities[,] secondTurn = new SingleTurnProbabilities[,]
        {
            {new SingleTurnProbabilities(Card.MAX_CARD_SCORE * 2), new SingleTurnProbabilities(Card.MAX_CARD_SCORE * 2)},
            {new SingleTurnProbabilities(Card.MAX_CARD_SCORE * 2), new SingleTurnProbabilities(Card.MAX_CARD_SCORE * 2)},
        };

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

