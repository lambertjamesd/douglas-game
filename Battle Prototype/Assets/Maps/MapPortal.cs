using UnityEngine;
using System.Collections;

public class MapPortal : MonoBehaviour {
	public MapPath target;

	public void OnTriggerEnter2D(Collider2D collider) {
		if (collider.gameObject.CompareTag("Player")) {
			GameObject current = gameObject;

			WorldController worldController = null;

			while (worldController == null && current != null) {
				worldController = current.GetComponent<WorldController>();

				if (current.transform.parent != null) {
					current = current.transform.parent.gameObject;
				} else {
					current = null;
				}
			}

			if (worldController != null) {
				worldController.Goto(target);
			}
		}
	}
}
