﻿using System.Collections;
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

    public static float MONEY_ANIMATE_TIME = 0.25f;

    public IEnumerable<Card> UnplayedCards()
    {
        return hand.Where((card) => card != null);
    }

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

    public IEnumerator PutCardOnTable(Card card)
    {
        int handIndex = hand.IndexOf(card);

        if (handIndex != -1 && card != null)
        {
            handSlots[handIndex].gameObject.SetActive(false);
            int boardSlot = playedCards.Count + (boardSlots[playedCards.Count].gameObject.activeSelf ? 1 : 0);
            boardSlots[boardSlot].gameObject.SetActive(true);
            boardSlots[boardSlot].sprite = cardBack;
            yield return TweenHelper.LerpPosition(handSlots[handIndex].transform.position, boardSlots[boardSlot].transform.position, MONEY_ANIMATE_TIME, (pos) => boardSlots[boardSlot].transform.position = pos);
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

    public void RevealHand()
    {
        for (int i = 0; i < hand.Count; ++i)
        {
            if (hand[i] != null)
            {
                handSlots[i].sprite = hand[i].sprite;
            }
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

    public int GetMaxScore()
    {
        return CardAIBase.IdealScore(UnplayedCards(), playedCards);
    }

    public bool ShowedDouble()
    {
        return playedCards.Count < 2 || playedCards[0].suite == playedCards[1].suite;
    }
}
