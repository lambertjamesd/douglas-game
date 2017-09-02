using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TweenHelper {
    public delegate void TweenCallback(float t);
    public static IEnumerator Tween(float duration, TweenCallback callback)
    {
        float remaining = duration;

        while (remaining > 0.0)
        {
            callback(1.0f - remaining / duration);
            remaining -= Time.deltaTime;
            yield return null;
        }

        callback(1.0f);
    }

    public delegate void PositionCallback(Vector3 pos);
    public static IEnumerator LerpPosition(Vector3 start, Vector3 end, float duration, PositionCallback callback)
    {
        return Tween(duration, (t) =>
        {
            callback(Vector3.Lerp(start, end, t));
        });
    }
}
