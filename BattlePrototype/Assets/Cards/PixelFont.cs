using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelFont : MonoBehaviour {
    public Font font;

	void Start () {
        font.material.mainTexture.filterMode = FilterMode.Point;
	}
}
