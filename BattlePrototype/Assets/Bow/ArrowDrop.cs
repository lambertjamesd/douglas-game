using UnityEngine;
using System.Collections;

public class ArrowDrop : MonoBehaviour {
	public int arrowCount = 50;
    public AmmoType type = AmmoType.Colt45;

	public void OnTriggerEnter2D(Collider2D collider) {
		InventorySlot inventory = collider.GetComponent<InventorySlot>();

		if (inventory != null) {
			inventory.GiveAmmo(type, arrowCount);
			Destroy(gameObject);
		}
	}
}
