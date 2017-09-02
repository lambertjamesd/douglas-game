using UnityEngine;
using System.Collections;

public class ZOffset : MonoBehaviour {
	private static float ZRatio = 0.1f;

	void LateUpdate () {
		Vector3 position = transform.position;
		position.z = position.y * ZRatio;
		transform.position = position;
	}
}
