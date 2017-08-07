using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathSequence : MonoBehaviour
{

    public static IEnumerator Start()
    {
        Time.timeScale = 0.0f;
        yield return new WaitForSecondsRealtime(2.0f);
        GameObject.FindGameObjectWithTag("GameController").GetComponent<WorldController>().SwitchTo(MapDirections.Dead);
        Time.timeScale = 1.0f;
    }
}
