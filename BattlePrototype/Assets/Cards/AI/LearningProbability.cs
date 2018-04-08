using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace shootout
{
    public class LearningProbability
    {
        private float[,] probabilityMap;

        public LearningProbability(int myMaxPoints, int theirMaxPoints)
        {
            probabilityMap = new float[myMaxPoints + 1, theirMaxPoints + 1];
        }

        public float GetProbability(int myScore, int theirScore)
        {
            return probabilityMap[myScore, theirScore];
        }

        public void UpdateProbability(int myScore, int theirScore, bool isWin, float bias)
        {
            probabilityMap[myScore, theirScore] = probabilityMap[myScore, theirScore] * (1.0f - bias) + (isWin ? bias : 0.0f);
        }

        public void SetProbability(int myScore, int theirScore, float value)
        {
            probabilityMap[myScore, theirScore] = value;
        }

        public void CopyFrom(LearningProbability other)
        {
            for (int i = 0; i < Mathf.Min(probabilityMap.GetLength(0), other.probabilityMap.GetLength(0)); ++i)
            {
                for (int j = 0; j < Mathf.Min(probabilityMap.GetLength(1), other.probabilityMap.GetLength(1)); ++i)
                {
                    probabilityMap[i, j] = other.probabilityMap[i, j];
                }
            }
        }

        public void Write(BinaryWriter output)
        {
            int myMaxScore = probabilityMap.GetLength(0);
            int theirMaxScore = probabilityMap.GetLength(1);

            output.Write((short)myMaxScore);
            output.Write((short)theirMaxScore);

            for (int myScore = 0; myScore < myMaxScore; ++myScore)
            {
                for (int theirScore = 0; theirScore < theirMaxScore; ++theirScore)
                {
                    output.Write(probabilityMap[myScore, theirScore]);
                }
            }
        }

        public void Read(BinaryReader input)
        {
            int myMaxScore = input.ReadInt16();
            int theirMaxScore = input.ReadInt16();

            probabilityMap = new float[myMaxScore, theirMaxScore];
            
            for (int myScore = 0; myScore < myMaxScore; ++myScore)
            {
                for (int theirScore = 0; theirScore < theirMaxScore; ++theirScore)
                {
                    probabilityMap[myScore, theirScore] = input.ReadSingle();
                }
            }
        }
    }
}

