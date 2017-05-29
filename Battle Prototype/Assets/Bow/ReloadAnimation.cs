using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadAnimation : MonoBehaviour {
    public SpriteRenderer[] bullets;

    public void SetShotCount(int count)
    {
        for (int i = 0; i < bullets.Length; ++i)
        {
            bullets[i].enabled = i < count;
        }
    }
}
