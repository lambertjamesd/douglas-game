using UnityEngine;
using System.Collections;

public class DropAnimation : MonoBehaviour {
	public Transform dropTransform;

	public void Throw(Vector3 endPosition, float time) {
		StartCoroutine(ThrowDrop(this, endPosition, time));
	}

	public static IEnumerator ThrowDrop(DropAnimation target, Vector3 endPosition, float time) {
		Collider2D collider = target.GetComponent<Collider2D>();
		
		if (collider != null) {
			collider.enabled = false;
		}
		
		IEnumerator other = PhysicsUtil.MoveInLine(target.transform, endPosition, time);
		IEnumerator height = PhysicsUtil.MoveInArc(target.dropTransform, new Vector3(0.0f, 0.0f, 0.0f), time, true);
		
		while (other.MoveNext() && height.MoveNext()) {
			yield return null;
		}
		
		
		if (collider != null) {
			collider.enabled = true;
		}
	}
}
