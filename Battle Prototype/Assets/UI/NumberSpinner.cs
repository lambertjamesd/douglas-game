using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberSpinner : MonoBehaviour
{
    public int value;
    public int minValue = 0;
    public int maxValue = int.MaxValue;

    public delegate void OnChange();

    public List<NumberSpinnerDigit> digits = new List<NumberSpinnerDigit>();

    private List<OnChange> changeListeners = new List<OnChange>();

    public void onChange(OnChange change)
    {
        changeListeners.Add(change);
    }

    public void SetMin(int value)
    {
        this.minValue = value;
        this.SetValue(this.value);
    }

    public void SetMax(int value)
    {
        this.maxValue = value;
        this.SetValue(this.value);
    }

    public void SetValue(int value)
    {
        this.value = System.Math.Min(maxValue, System.Math.Max(minValue, value));
        changeListeners.ForEach(listener => listener());
    }

    public void SetEnabled(bool value)
    {
        if (digits != null)
        {
            foreach (NumberSpinnerDigit digit in digits)
            {
                digit.SetEnabled(value);
            }
        }
    }
}
