using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class CardProbability {
    public static float[] scoreProbability = new float[]{
        0f,
        0f,
        0f,
        0f,
        0f,
        0f,
        0f,
        0f,
        1.20E-05f,
        2.80E-05f,
        9.20E-05f,
        0.000213f,
        0.000407f,
        0.000695f,
        0.001201f,
        0.001849f,
        0.002862f,
        0.004142f,
        0.005745f,
        0.007926f,
        0.010554f,
        0.013464f,
        0.01782f,
        0.02199f,
        0.02682f,
        0.032182f,
        0.037292f,
        0.042771f,
        0.048613f,
        0.053879f,
        0.058198f,
        0.061458f,
        0.063532f,
        0.064522f,
        0.063801f,
        0.061285f,
        0.057111f,
        0.051368f,
        0.043891f,
        0.036378f,
        0.028069f,
        0.021261f,
        0.014193f,
        0.010671f,
        0.008797f,
        0.007091f,
        0.005519f,
        0.004289f,
        0.003013f,
        0.002126f,
        0.001416f,
        0.000819f,
        0.000416f,
        0.000219f,
    };

    public static float[] winProbability = new float[]{
        0f,
        0f,
        0f,
        0f,
        0f,
        0f,
        0f,
        0f,
        0f,
        0.000012f,
        0.00004f,
        0.000132f,
        0.000345f,
        0.000752f,
        0.001447f,
        0.002648f,
        0.004497f,
        0.007359f,
        0.011501f,
        0.017246f,
        0.025172f,
        0.035726f,
        0.04919f,
        0.06701f,
        0.089f,
        0.11582f,
        0.148002f,
        0.185294f,
        0.228065f,
        0.276678f,
        0.330557f,
        0.388755f,
        0.450213f,
        0.513745f,
        0.578267f,
        0.642068f,
        0.703353f,
        0.760464f,
        0.811832f,
        0.855723f,
        0.892101f,
        0.92017f,
        0.941431f,
        0.955624f,
        0.966295f,
        0.975092f,
        0.982183f,
        0.987702f,
        0.991991f,
        0.995004f,
        0.99713f,
        0.998546f,
        0.999365f,
        0.999781f,
    };

    public static float ProbabilityOfWin(int myPoints, IEnumerable<Card> visibleCards, IEnumerable<Card> oponentPlayedCards)
    {
        int sampleCount = 256;

        Deck testDeck = new Deck(null);
        testDeck.Remove(visibleCards);

        int remainingCardCount = 5 - oponentPlayedCards.Count();

        int numberOfWins = 0;

        for (int i = 0; i < sampleCount; ++i)
        {
            testDeck.Shuffle();
            var remainingCards = testDeck.Deal(remainingCardCount);

            int handScore = CardAIBase.ScoreHand(CardAIBase.IdealHand(remainingCards, oponentPlayedCards));

            if (handScore > myPoints)
            {
                ++numberOfWins;
            }
        }

        return (float)numberOfWins / sampleCount;
    }
}
