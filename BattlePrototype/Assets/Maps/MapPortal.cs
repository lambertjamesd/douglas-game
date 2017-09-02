using UnityEngine;
using System.Collections;

public class MapPortal : MonoBehaviour {
	public MapPath target;
    public Collider2D boxCollider;

    public void Start()
    {
        boxCollider = GetComponent<Collider2D>();
    }

    public void OnTriggerStay2D(Collider2D collider) {
        Bounds bounds = collider.bounds;
        Bounds thisBounds = boxCollider.bounds;
        thisBounds.Expand(new Vector3(0.0f, 0.0f, 1000000.0f));
    
        if (collider.gameObject.CompareTag("Player") && thisBounds.Contains(bounds.min) && thisBounds.Contains(bounds.max)) {
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
