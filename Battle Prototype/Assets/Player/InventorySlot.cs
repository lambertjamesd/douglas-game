using UnityEngine;
using System.Collections;

public class InventorySlot : MonoBehaviour 
{
	public Inventory inventory;
	public InventoryItem primaryItem;

	public State usePrimaryItem() 
    {
		if (primaryItem != null && primaryItem.canUse(inventory)) {
			return primaryItem.useItem(inventory);
		} else {
			return null;
		}
	}

    public State checkUseItems()
    {
        State result = null;

        if (result == null && Input.GetButtonDown("Fire1"))
        {
            result = usePrimaryItem();
        }

        return result;
    }

	public void GiveArrows(int count) 
    {
		inventory.arrows += count;
	}
}
