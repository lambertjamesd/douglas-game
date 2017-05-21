using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ink.Runtime;

public class TextOption : MonoBehaviour {
    public Text text;
    public Image selectionIndicator;

    public void UseChoice(Choice choice)
    {
        text.text = choice.text;
    }

    public void SetSelected(bool selected)
    {
        selectionIndicator.gameObject.SetActive(selected);
    }
}
