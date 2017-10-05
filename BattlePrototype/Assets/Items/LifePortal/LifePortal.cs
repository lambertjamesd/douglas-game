using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifePortal : MonoBehaviour {
    public void OnTriggerEnter2D(Collider2D collision)
    {
        StoryManager.GetSingleton().SetBoolean("player_is_dead", false);
        GameObject.FindGameObjectWithTag("GameController").GetComponent<WorldController>().SwitchTo(MapDirections.Living);
    }
}
