using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxisToButton {
    private static float tolerance = 0.7f;
    private float lastValue = 0.0f;
    private float currentValue = 0.0f;

    public void Update(float value)
    {
        lastValue = currentValue;
        currentValue = value;
    }

    public bool GetPositiveDown()
    {
        return currentValue >= tolerance && lastValue < tolerance;
    }

    public bool GetNegativeDown()
    {
        return currentValue <= -tolerance && lastValue > -tolerance;
    }
}
