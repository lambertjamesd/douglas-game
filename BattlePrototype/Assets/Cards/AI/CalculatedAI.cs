using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CalculatedAI : CardAIBase
{
    shootout.CardGameAI ai;

    public CalculatedAI(int index, PlayerHand hand, Text moneyLabel, shootout.CardGameAI ai)
        : base(index, hand, moneyLabel)
    {
        this.ai = ai;
    }

    public override TurnChoice ChooseCard(IEnumerable<Card> myShowing, IEnumerable<Card> theirShowing, int inPot, int currentBid, int currentBidScalar, int currentTurn)
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
        shootout.TurnResult result = state.CaclulateOptimalTurn(ai);
        TurnChoice choice = new TurnChoice(result.bid, result.chosenCard);
        choice.extraCard = result.fourthCard;
        return choice;
    }
}
