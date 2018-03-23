using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextDebugger : MonoBehaviour {
    public Text text;

    private static TextDebugger currentDebugger;

	// Use this for initialization
	void Start ()
    {
        currentDebugger = this;
        text.text = "";
	}
	
    public static void Log(string message)
    {
        if (currentDebugger != null)
        {
            currentDebugger.text.text = message + "\n" + (currentDebugger.text.text ?? "");
        }
    }
}
