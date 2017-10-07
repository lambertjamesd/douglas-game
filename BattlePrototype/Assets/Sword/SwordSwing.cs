using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SwordSwing : MonoBehaviour {
	public SwingFrame[] frames = null;

	private HashSet<GameObject> hitObjects = null;
	private SwingFrame.HitCallback currentCallback = null;

	// Use this for initialization
	void Start () {
		if (frames != null) {
			for (int i = 0; i < frames.Length; ++i) {
				frames[i].hitCallback = OnHit;
			}
		}
	}
	
	public IEnumerator Swing(SwingFrame.HitCallback callback){
		if (currentCallback == null) {
			hitObjects = new HashSet<GameObject>();
			currentCallback = callback;

			for (int i = 0; i < frames.Length; ++i) {
				SwingFrame current = frames[i];
				current.gameObject.SetActive(true);
                yield return AsyncUtil.Pause(current.time);
				current.gameObject.SetActive(false);
			}

			currentCallback = null;
		}
	}

	private void OnHit(Collider2D collider) {
		if (currentCallback != null && !hitObjects.Contains(collider.gameObject)) {
			hitObjects.Add(collider.gameObject);
			currentCallback(collider);
		}
	}
}
