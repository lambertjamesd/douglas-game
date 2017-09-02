using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimePause {
    private static List<float> scalars = new List<float>();
    private static float uniformScale = 1.0f;

    private static void UpdateTime()
    {
        float result = uniformScale;
        foreach (float a in scalars)
        {
            result *= a;
        }
        Time.timeScale = result;
    }

    public static void SetUniformScale(float amount)
    {
        uniformScale = amount;
        UpdateTime();
    }

    public static void ScaleTime(float amount)
    {
        scalars.Add(amount);
        UpdateTime();
    }

    public static void UnscaleTime(float amount)
    {
        scalars.Remove(amount);
        UpdateTime();
    }
}
