using UnityEngine;
using System.Collections;

public abstract class InventoryItem : MonoBehaviour {
	public State state;

	public abstract bool canUse(InventorySlot inventory);

	public abstract State useItem(InventorySlot inventory);
}
