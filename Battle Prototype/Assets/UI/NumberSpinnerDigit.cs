using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberSpinnerDigit : MonoBehaviour
{
    public NumberSpinner forSpinner;
    public int digit;
    private int increment = 1;

    public Button up;
    public Button down;
    public Text text;

    void Start()
    {
        increment = 1;

        while (digit > 0)
        {
            increment *= 10;
            --digit;
        }

        up.onClick.AddListener(Increment);

        down.onClick.AddListener(Decrement);

        forSpinner.onChange(UpdateText);
        UpdateText();
    }

    public void Increment()
    {
        forSpinner.SetValue(forSpinner.value + ((forSpinner.value + increment) % (increment * 10)) - forSpinner.value % (increment * 10));
    }

    public void Decrement()
    {
        forSpinner.SetValue(forSpinner.value + ((forSpinner.value + increment * 9) % (increment * 10)) - forSpinner.value % (increment * 10));
    }

    public void UpdateText()
    {
        text.text = ((forSpinner.value / increment) % 10).ToString();
    }
}
