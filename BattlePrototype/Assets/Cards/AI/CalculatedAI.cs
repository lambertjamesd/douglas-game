using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.IO;

public class CalculatedAI : CardAIBase
{
    shootout.CardGameAI ai;

    public CalculatedAI(int index, PlayerHand hand, Text moneyLabel, shootout.CardGameAI ai)
        : base(index, hand, moneyLabel)
    {
        this.ai = ai;
    }

    public override shootout.TurnResult ChooseCard(IEnumerable<Card> myShowing, IEnumerable<Card> theirShowing, int inPot, int currentBid, int currentBidScalar, int currentTurn)
    {
        shootout.CardGameState state = new shootout.CardGameState(
            CardAIBase.IdealHand(hand.UnplayedCards(), myShowing).Except(myShowing), 
            myShowing, 
            theirShowing,
            currentBidScalar == 0, 
            currentBidScalar == 0,
            inPot, 
            Mathf.Max(currentBid, 0), 
            currentBidScalar
        );
        shootout.CardGameState.checkCount = 0;
        return state.CaclulateOptimalTurn(ai);
    }

    public override void Learn(List<shootout.TurnResult> turnResults, bool amIFirst, int myTopScore, int theirTopScore, bool myDouble, bool theirDouble)
    {
        if (ai.learn != null)
        {
            ai.learn(turnResults, amIFirst, myTopScore, theirTopScore, myDouble, theirDouble);
        }
    }
}
