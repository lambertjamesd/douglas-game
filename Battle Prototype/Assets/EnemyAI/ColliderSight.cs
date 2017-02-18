using UnityEngine;
using System.Collections;

public class ColliderSight : Sight {
	private Collider2D currentTarget;
	
	public override Collider2D GetVisibleObject() {
		return currentTarget;
	}

	public void OnTriggerStay2D(Collider2D collider) {
		if (currentTarget == null) {
			if (canSeeObject(collider.gameObject)) {
				currentTarget = collider;
			}
		}
	}

	public void OnTriggerExit2D(Collider2D collider) {
		if (collider == currentTarget) {
			currentTarget = null;
		}
	}
}
