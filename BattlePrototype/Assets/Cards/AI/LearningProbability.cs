using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace shootout
{
    public class LearningProbability
    {
        private float[,] probabilityMap;

        public LearningProbability(int maxPoints)
        {
            probabilityMap = new float[maxPoints + 1, maxPoints + 1];
        }

        public float GetProbability(int myScore, int theirScore)
        {
            return probabilityMap[myScore, theirScore];
        }

        public void UpdateProbability(int myScore, int theirScore, bool isWin, float bias)
        {
            probabilityMap[myScore, theirScore] = probabilityMap[myScore, theirScore] * (1.0f - bias) + (isWin ? bias : 0.0f);
        }
    }
}

