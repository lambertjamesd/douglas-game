using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalculatedAI : CardAIBase
{
    public CalculatedAI(int index, PlayerHand hand, Text moneyLabel)
        : base(index, hand, moneyLabel)
    {

    }

    public override float PlayerFoldProbability(IEnumerable<Card> showingCards, IEnumerable<Card> oponentShowingCards, int turn, int bid, int inPot, bool isOpening)
    {
        return 0.0f;
    }

    public override float WinProbability(int bid, int score)
    {
        return base.WinProbability(bid, score);
    }
}
