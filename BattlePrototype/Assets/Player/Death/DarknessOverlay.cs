using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarknessOverlay : MonoBehaviour {

    static DarknessOverlay currentOverlay;

    public void Start()
    {
        gameObject.SetActive(false);
        currentOverlay = this;
    }

    public SpriteRenderer spriteRenderer;
    public Camera targetCamera;

    public void UpdateValues(Vector3 worldCenter, float worldRadius)
    {
        transform.localScale = new Vector3(targetCamera.orthographicSize * targetCamera.aspect * 2.0f, targetCamera.orthographicSize * 2.0f, 1.0f);
        Vector3 cameraCenter = targetCamera.WorldToViewportPoint(worldCenter);
        spriteRenderer.material.SetVector("_CircleOriginRadius", new Vector4(cameraCenter.x, cameraCenter.y, worldRadius * 0.5f / targetCamera.orthographicSize, targetCamera.aspect));
    }

    public static DarknessOverlay GetOverlay()
    {
        return currentOverlay;
    }
}
