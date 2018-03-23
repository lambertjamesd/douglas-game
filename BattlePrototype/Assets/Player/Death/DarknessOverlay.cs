using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarknessOverlay : MonoBehaviour {

    static DarknessOverlay currentOverlay;

    private int currentAnimation = 0;
    private int activeAnimationCount = 0;

    public int StartAnimation()
    {
        if (activeAnimationCount == 0)
        {
            gameObject.SetActive(true);
        }

        ++activeAnimationCount;

        return ++currentAnimation;
    }

    public void EndAnimation(int animationIndex)
    {
        --activeAnimationCount;

        if (activeAnimationCount == 0)
        {
            gameObject.SetActive(false);
        }
    }

    public void Start()
    {
        gameObject.SetActive(false);
        currentOverlay = this;
    }

    public SpriteRenderer spriteRenderer;
    public Camera targetCamera;

    public void UpdateValues(Vector3 worldCenter, float worldRadius, int animationIndex)
    {
        if (animationIndex == currentAnimation)
        {
            transform.localScale = new Vector3(targetCamera.orthographicSize * targetCamera.aspect * 2.0f, targetCamera.orthographicSize * 2.0f, 1.0f);
            Vector3 cameraCenter = targetCamera.WorldToViewportPoint(worldCenter);
            spriteRenderer.material.SetVector("_CircleOriginRadius", new Vector4(cameraCenter.x, cameraCenter.y, worldRadius * 0.5f / targetCamera.orthographicSize, targetCamera.aspect));
        }
    }

    public void SetColor(Color color, int animationIndex)
    {
        if (animationIndex == currentAnimation)
        {
            spriteRenderer.material.color = color;
        }
    }

    public static DarknessOverlay GetOverlay()
    {
        return currentOverlay;
    }
}
