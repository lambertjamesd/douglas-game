using UnityEngine;
using System.Collections;

[System.Serializable]
public class DropSlot {
	public float probability = 1.0f;
	public GameObject drop;
	public Vector3 startOffset;
	public Vector3 endOffset;
	public float dropTime = 0.0f;
}

public class ItemDrop : MonoBehaviour {
	public DropSlot[] drops;

	void SpawnDrop(DropSlot slot) {
		if (slot.probability >= Random.value) {
			GameObject drop = (GameObject)Instantiate(slot.drop, transform.position + slot.startOffset, Quaternion.identity);

			if (slot.dropTime > 0.0f) {
				DropAnimation anim = drop.GetComponent<DropAnimation>();
				anim.Throw(transform.position + slot.endOffset, slot.dropTime);
			}
		}
	}
	
	public void OnDestroy() {
		for (int i = 0; drops != null && i < drops.Length; ++i) {
			SpawnDrop(drops[i]);
		}
	}
}
