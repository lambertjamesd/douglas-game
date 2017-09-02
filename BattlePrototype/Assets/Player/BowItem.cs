using UnityEngine;
using System.Collections;

public class BowItem : InventoryItem {
    public Gun gun;

	public override bool canUse (InventorySlot inventory) {
		return inventory.GetAmmoCount(gun.gunStats.type) > 0 || gun.GetShotsLeft() > 0;
	}

	public override State useItem (InventorySlot inventory) {
		return state;
	}
}
