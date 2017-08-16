using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PlayerHand : MonoBehaviour {
    public Image[] boardSlots;
    public Image[] handSlots;
    public bool isHidden = false;
    public Text pointsShown;
    private Sprite cardBack;

    private List<Card> hand = new List<Card>();
    private List<Card> playedCards = new List<Card>();

    void Start()
    {
        foreach (Image boardSlot in boardSlots)
        {
            boardSlot.gameObject.SetActive(false);
        }

        foreach (Image handSlot in handSlots)
        {
            handSlot.gameObject.SetActive(false);
        }
    }

    public void UseBack(Sprite back)
    {
        cardBack = back;
    }

    public void GiveHand(List<Card> hand)
    {
        this.hand = hand;

        for (int i = 0; i < hand.Count; ++i)
        {
            handSlots[i].sprite = isHidden ? cardBack : hand[i].sprite;
            handSlots[i].gameObject.SetActive(true);
        }
    }

    public List<Card> GetHand()
    {
        return hand;
    }

    public List<Card> GetPlayedCards()
    {
        return playedCards;
    }

    public int GetScore()
    {
        int sum = 0;

        foreach (Card playedCard in playedCards)
        {
            sum += playedCard.PointValue();
        }

        return sum;
    }

    public void PlayCard(Card card)
    {
        int index = hand.IndexOf(card);

        if (index >= 0 && index < hand.Count && hand[index] != null)
        {
            boardSlots[playedCards.Count].sprite = hand[index].sprite;
            boardSlots[playedCards.Count].gameObject.SetActive(true);
            playedCards.Add(hand[index]);

            hand[index] = null;
            handSlots[index].gameObject.SetActive(false);
            pointsShown.text = GetScore().ToString();
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

        pointsShown.text = "0";
        hand.Clear();
        playedCards.Clear();
        return result;
    }
}
