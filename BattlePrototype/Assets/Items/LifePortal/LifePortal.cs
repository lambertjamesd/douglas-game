using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifePortal : MonoBehaviour {
    public void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<WorldController>().SwitchTo(MapDirections.Living);
    }
}
