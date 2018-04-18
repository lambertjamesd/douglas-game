using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace shootout
{
    public class ProbabilityTracker
    {
        private ushort didHappen = 0;
        private ushort didNotHappen = 0;

        public ProbabilityTracker()
        {

        }

        public ProbabilityTracker(float from)
        {
            Set(from);
        }

        public float Get()
        {
            return (float)didHappen / ((uint)didHappen + didNotHappen);
        }

        public void Set(float value)
        {
            if (value >= 1.0f)
            {
                didNotHappen = 0;
                didHappen = 0x8000;
            }
            else if (value > 0.5f)
            {
                didNotHappen = 0x8000;
                didHappen = (ushort)((didNotHappen * value) / (1.0f - value));
            }
            else if (value <= 0.0f)
            {
                didHappen = 0x8000;
                didNotHappen = 0;
            }
            else
            {
                didHappen = 0x8000;
                didNotHappen = (ushort)(didHappen * (1.0f - value) / value);
            }
        }

        public void Update(bool didSucceed, ushort weight)
        {
            if (didSucceed)
            {
                int check = didHappen + weight;

                if (check >= 0x10000)
                {
                    didHappen = (ushort)(check / 2);
                    didNotHappen /= 2;
                }
            }
            else
            {
                int check = didNotHappen + weight;

                if (check >= 0x10000)
                {
                    didNotHappen = (ushort)(check / 2);
                    didHappen /= 2;
                }
            }
        }
    }
}
