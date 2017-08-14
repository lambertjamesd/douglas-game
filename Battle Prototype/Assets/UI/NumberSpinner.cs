using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberSpinner : MonoBehaviour
{
    public int value;

    public delegate void OnChange();

    private List<OnChange> changeListeners = new List<OnChange>();

    public void onChange(OnChange change)
    {
        changeListeners.Add(change);
    }

    public void SetValue(int value)
    {
        this.value = value;
        changeListeners.ForEach(listener => listener());
    }
}
