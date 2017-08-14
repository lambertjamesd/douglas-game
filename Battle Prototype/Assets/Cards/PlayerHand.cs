using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PlayerHand : MonoBehaviour {
    public Image[] boardSlots;
    public Image[] handSlots;

    private List<Card> hand;
    private List<Card> playedCards;

    public void GiveHand(List<Card> hand)
    {
        this.hand = hand;

        for (int i = 0; i < hand.Count; ++i)
        {
            handSlots[i].sprite = hand[i].sprite;
            handSlots[i].gameObject.SetActive(true);
        }
    }

    public List<Card> GetHand()
    {
        return hand;
    }

    public void PlayCard(int index)
    {
        if (index < hand.Count && hand[index] != null)
        {
            boardSlots[playedCards.Count].sprite = hand[index].sprite;
            boardSlots[playedCards.Count].gameObject.SetActive(true);
            playedCards.Add(hand[index]);

            hand[index] = null;
            handSlots[index].gameObject.SetActive(false);
        }
    }

    public IEnumerable<Card> TakeCards()
    {
        Card[] result = hand.Union(playedCards).Where(a => a != null).ToArray();

        foreach (Image boardSlot in boardSlots)
        {
            boardSlot.gameObject.SetActive(false);
        }

        foreach (Image handSlot in handSlots)
        {
            handSlot.gameObject.SetActive(false);
        }

        hand.Clear();
        playedCards.Clear();
        return result;
    }
}
