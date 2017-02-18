using UnityEngine;
using System.Collections;

public class InventoryDisplay : MonoBehaviour {
	public CustomFontRenderer fontRenderer;
	public InventorySlot inventory;
	public Transform arrowCount;

	void Update() {
		fontRenderer.DrawText(arrowCount.position, inventory.inventory.arrows.ToString());
	}
}
