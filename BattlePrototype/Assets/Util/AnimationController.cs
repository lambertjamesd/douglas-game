using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour {
    public Animator unityAnimator;
    public SpritesheetAnimator spritesheetAnimator;

    public void SetUseUnscaledTime(bool value)
    {
        if (unityAnimator != null)
        {
            unityAnimator.updateMode = value ? AnimatorUpdateMode.UnscaledTime : AnimatorUpdateMode.Normal;
        }

        if (spritesheetAnimator != null)
        {
            spritesheetAnimator.useUnscaledTime = value;
        }
    }

    public void SetFloat(string name, float value)
    {
        if (unityAnimator != null)
        {
            unityAnimator.SetFloat(name, value);
        }

        if (spritesheetAnimator != null)
        {
            spritesheetAnimator.SetValue(name, value);
        }
    }

    public void SetInteger(string name, int value)
    {
        if (unityAnimator != null)
        {
            unityAnimator.SetInteger(name, value);
        }

        if (spritesheetAnimator != null)
        {
            spritesheetAnimator.SetValue(name, value);
        }
    }

    public void SetBool(string name, bool value)
    {
        if (unityAnimator != null)
        {
            unityAnimator.SetBool(name, value);
        }

        if (spritesheetAnimator != null)
        {
            spritesheetAnimator.SetValue(name, value ? 1.0f : 0.0f);
        }
    }
}
