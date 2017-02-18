using UnityEngine;
using System.Collections;

public abstract class InventoryItem : MonoBehaviour {
	public State state;

	public abstract bool canUse(Inventory inventory);

	public abstract State useItem(Inventory inventory);
}
