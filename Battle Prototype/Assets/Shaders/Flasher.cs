using UnityEngine;
using System.Collections;

public class Flasher : MonoBehaviour {
	public Gradient flashGradient;
	public float frequency = 1.0f;
	public float depth = 1.0f;
	public Renderer target = null;
    public Color defaultColor = Color.white;

	private float currentTime = 0.0f;

	public void Update() {
		currentTime += Time.deltaTime * frequency * Mathf.PI * 2.0f;
		float currentPostion = (-Mathf.Cos(currentTime) * 0.5f + 0.5f) * depth;
		target.material.color = flashGradient.Evaluate(currentPostion);
	}

	void OnDisable() {
		target.material.color = defaultColor;
	}
}
