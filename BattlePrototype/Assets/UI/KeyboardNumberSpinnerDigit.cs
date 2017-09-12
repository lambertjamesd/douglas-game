using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardNumberSpinnerDigit : MonoBehaviour {
    public Image highlightImage;
    public Text label;
    public Color hightlightColor;
    public RectTransform rectTransform;

    public void SetHighlighted(bool value)
    {
        highlightImage.color = value ? hightlightColor : Color.clear;
    }
}
