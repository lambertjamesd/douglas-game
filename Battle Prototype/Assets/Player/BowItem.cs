using UnityEngine;
using System.Collections;

public class BowItem : InventoryItem {
    public Gun gun;

	public override bool canUse (Inventory inventory) {
		return inventory.arrows > 0 || gun.shotsLeft > 0;
	}

	public override State useItem (Inventory inventory) {
		return state;
	}
}
