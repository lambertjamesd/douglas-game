using UnityEngine;
using System.Collections;

public class BowItem : InventoryItem {
	public override bool canUse (Inventory inventory) {
		return inventory.arrows > 0;
	}

	public override State useItem (Inventory inventory) {
		return state;
	}
}
