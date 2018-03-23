using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AILogic {
    public IEnumerable<Card> ChooseCards(IEnumerable<Card> hand)
    {
        return CardAIBase.IdealHand(hand, new Card[] { });
    }

    public virtual float OponentFoldProbability(IEnumerator<Card> myCardsShowing, IEnumerator<Card> oponentCardsShowin, int myBid, int moneyInPot)
    {
        return 0.0f;
    }

    public virtual float WinProbability(int myScore, int currentOponentScore, bool isFirst, int cardsPlayed, bool suitesMatch, int minBid, int moneyInPot)
    {
        return 1.0f;
    }
}
