using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGameLogic : MonoBehaviour {
    public DeckSkin deckSkin;
    private Deck deck;

	// Use this for initialization
	void Start () {
        deck = new Deck(deckSkin);
	}
}
