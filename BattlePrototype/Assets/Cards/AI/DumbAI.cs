using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DumbCardAIPlayer : CardAIBase
{
    public DumbCardAIPlayer(int index, PlayerHand hand, Text moneyLabel)
        : base(index, hand, moneyLabel)
    {

    }
}

public class FoldAI : CardAIBase
{       
    public FoldAI(int index, PlayerHand hand, Text moneyLabel)
        : base(index, hand, moneyLabel)
    {

    }

    public override float WinProbability(int bid, int score)
    {
 	     return CardProbability.winProbability[score];
    }
}