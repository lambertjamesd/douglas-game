using UnityEngine;
using System.Collections;

public class Sight : MonoBehaviour {
	public LayerMask canSee;

	public virtual Collider2D GetVisibleObject() {
		return null;
	}

	public bool canSeeObject(GameObject gameObject) {
		ObjectVisibility visibility = gameObject.GetComponent<ObjectVisibility>();

		if (visibility != null) {
			return (canSee & visibility.visibilityLayers) != 0;
		}

		return true;
	}
}
