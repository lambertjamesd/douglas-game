using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BankGUI : MonoBehaviour {
    public Text inHand;
    public Text inBank;
	
	// Update is called once per frame
	void Update () {
        StoryManager story = StoryManager.GetSingleton();
        inHand.text = "$" + story.GetInt("player_money");
        inBank.text = "$" + story.GetInt("player_bank_money");
	}
}
