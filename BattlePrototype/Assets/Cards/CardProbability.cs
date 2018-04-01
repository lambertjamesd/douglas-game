using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

public static class CardProbability {
    public static float averageScore = 32.12842f;

    public static int MinScore = 7;
    public static int MaxScore = 53;

    public static float[] averageScoreFirstCard = new float[]
    {
        0f,
        0f,
        24.64319f,
        25.63318f,
        26.55847f,
        27.62708f,
        28.65527f,
        29.7218f,
        30.65833f,
        31.63696f,
        32.62732f,
        33.61963f,
        34.5675f,
        35.63867f,
        36.64722f,
    };

    public static float[] averageScoreSecondCard = new float[]
    {
        0f,
        0f,
        0f,
        0f,
        15.30481f,
        16.25928f,
        17.34204f,
        18.28784f,
        19.26099f,
        20.26624f,
        21.3125f,
        22.33752f,
        23.26575f,
        24.30139f,
        25.29431f,
        26.29346f,
        27.25842f,
        28.28601f,
        29.30408f,
        30.26331f,
        31.32471f,
        32.29602f,
        33.32495f,
        34.31689f,
        35.24829f,
        36.27405f,
        37.32288f,
        38.33203f,
        39.27783f,
    };

    public static float[] averageScoreSecondCardMatch = new float[]
    {
        0f,
        0f,
        0f,
        0f,
        19.4928f,
        20.57434f,
        21.48486f,
        22.46106f,
        23.38281f,
        24.53564f,
        25.57568f,
        26.53845f,
        27.6051f,
        28.54163f,
        29.67761f,
        30.55554f,
        31.53101f,
        32.41882f,
        33.56311f,
        34.55371f,
        35.49817f,
        36.51099f,
        37.49854f,
        38.48486f,
        39.51819f,
        40.51563f,
        41.52942f,
        42.55408f,
        43.50684f,
    };

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

    public static float CalculateProbabilityOfWin(int myScore, int theirShowingScore, int cardsPlayed, bool doesMatch)
    {
        Card[] playedCards;

        if (cardsPlayed == 1)
        {
            playedCards = new Card[] { new Card(null, Suite.Clubs, Card.PointsToType(theirShowingScore)) };
        }
        else if (cardsPlayed == 2)
        {
            playedCards = new Card[]
            {
                new Card(null,Suite.Clubs, Card.PointsToType(theirShowingScore / 2)),
                new Card(null,doesMatch ? Suite.Clubs : Suite.Diamonds, Card.PointsToType((theirShowingScore + 1) / 2)),
            };
        }
        else
        {
            throw new System.Exception("Invalid score setup");
        }

        return ProbabilityOfWin(myScore, new Card[] { }, playedCards);
    }

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

            testDeck.Discard(remainingCards);

            if (handScore < myPoints)
            {
                ++numberOfWins;
            }
        }

        return (float)numberOfWins / sampleCount;
    }

    public static float AverageHandScore(int sampleCount = 256)
    {
        float result = 0.0f;

        Deck testDeck = new Deck(null);

        for (int i = 0; i < sampleCount; ++i)
        {
            testDeck.Shuffle();
            Card[] restOfHand = testDeck.Deal(5).ToArray();

            result += CardAIBase.IdealScore(restOfHand, new Card[] { });

            testDeck.Discard(restOfHand);

        }

        return result / sampleCount;
    }

    public delegate void PointCallback(int amount);

    public static float AverageCardScore(int pointsShown, int cardsPlayed, bool suitesMatch, int sampleCount = 256)
    {
        float result = 0.0f;

        SamplePlayedCardsScore(pointsShown, cardsPlayed, suitesMatch, (sample) => result += sample, sampleCount);

        return result / sampleCount;
    }

    public static List<float> PointProbability(int pointsShown, int cardsPlayed, bool suitesMatch, int sampleCount = 256)
    {
        List<float> result = new List<float>();

        SamplePlayedCardsScore(pointsShown, cardsPlayed, suitesMatch, (sample) =>
        {
            while (result.Count <= sample)
            {
                result.Add(0.0f);
            }

            result[sample] += 1.0f / sampleCount;
        }, sampleCount);

        return result;
    } 

    public static void SamplePlayedCardsScore(int pointsShown, int cardsPlayed, bool suitesMatch, PointCallback callback, int sampleCount = 256)
    {
        int cardsToPlay = 3 - cardsPlayed;

        Deck testDeck = new Deck(null);

        for (int i = 0; i < sampleCount; ++i)
        {
            testDeck.Shuffle();
            Card[] restOfHand = testDeck.Deal(5 - cardsPlayed).ToArray();

            System.Array.Sort(restOfHand, (a, b) => b.PointValue() - a.PointValue());

            int rawPoints = restOfHand.Take(cardsToPlay).Select(card => card.PointValue()).Sum();

            int tripleWinstonPoints = 0;

            if (suitesMatch)
            {
                var tripleWinstonHand = restOfHand.Where(card => card.suite == Suite.Clubs).Take(cardsToPlay);
                tripleWinstonPoints = tripleWinstonHand.Select(card => card.PointValue()).Sum() + restOfHand.Where(card => !tripleWinstonHand.Contains(card)).Take(1).Select(card => card.PointValue()).Sum();
            }

            testDeck.Discard(restOfHand);
            
            callback(Mathf.Max(rawPoints, tripleWinstonPoints) + pointsShown);
        }
    }

    public static string PointProbabilityTable(int cardsPlayed, bool suitesMatch, int sampleCount = 256)
    {
        string result = "";

        for (int i = 0; i < 2 * cardsPlayed; ++i)
        {
            result += "0\n";
        }

        for (int i = 2 * cardsPlayed; i <= 14 * cardsPlayed; ++i)
        {
            result += string.Join(",", PointProbability(i, cardsPlayed, suitesMatch, sampleCount).Select(probabilty => probabilty.ToString()).ToArray()) + "\n";
        }

        return result;
    }
}
