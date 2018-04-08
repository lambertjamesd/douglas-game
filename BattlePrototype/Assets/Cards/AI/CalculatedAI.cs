using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        shootout.CardGameState state = new shootout.CardGameState(hand.UnplayedCards(), myShowing, theirShowing, currentBidScalar == 0, currentBidScalar == 0, inPot, Mathf.Max(currentBid, 0), currentBidScalar);

        shootout.TurnResult result = state.CaclulateOptimalTurn(ai);
        
        TurnChoice choice = new TurnChoice(result.bid, result.chosenCard);
        choice.extraCard = result.fourthCard;
        return choice;
    }
}
