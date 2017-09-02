using UnityEngine;
using System.Collections;

[System.Serializable]
public class RandomRange {
	public float min = 0.0f;
	public float max = 1.0f;

	public float GenerateValue() {
		return Random.Range(min, max);
	}
}

[System.Serializable]
public class RandomBoolean {
	public float trueProbability = 0.5f;

	public bool GenerateValue() {
		return Random.value <= trueProbability;
	}
}