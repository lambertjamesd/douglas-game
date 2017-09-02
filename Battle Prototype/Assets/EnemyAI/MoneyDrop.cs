using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyDrop : MonoBehaviour
{
    public int moneyAmount = 10;

    public void OnTriggerEnter2D(Collider2D collider)
    {
        StoryManager manager = StoryManager.GetSingleton();
        manager.SetInt("player_money", manager.GetInt("player_money") + moneyAmount);
        Destroy(gameObject);
    }
}
