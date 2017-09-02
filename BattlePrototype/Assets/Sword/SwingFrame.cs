using UnityEngine;
using System.Collections;

public class SwingFrame : MonoBehaviour {
	public Vector2[] points = null;
	public float time = 0.1f;
	public HitCallback hitCallback;

	public delegate void HitCallback(Collider2D collision);

	// Use this for initialization
	void Start () {
		PolygonCollider2D polygonCollider = GetComponent<PolygonCollider2D>();

		if (polygonCollider != null) {
			polygonCollider.points = points;
		}

		gameObject.SetActive(false);
	}

	void OnDrawGizmosSelected() {
		if (points != null && points.Length > 1) {
			Vector3 lastPoint = transform.TransformPoint(new Vector3(points[0].x, points[0].y));

			for (int i = 1; i < points.Length; ++i) {
				Vector3 nextPoint = transform.TransformPoint(new Vector3(points[i].x, points[i].y));
				Gizmos.DrawLine(lastPoint, nextPoint);
				lastPoint = nextPoint;
			}
		}
	}

	public void OnTriggerEnter2D(Collider2D collider) {
		if (this.hitCallback != null) {
			this.hitCallback(collider);
		}
	}
}
