using UnityEngine;
using System.Collections;

public abstract class InventorySlot : MonoBehaviour 
{
	public InventoryItem primaryItem;

    public abstract int GetAmmoCount(AmmoType type);
    public abstract void GiveAmmo(AmmoType type, int amount);

	public State usePrimaryItem() 
    {
		if (primaryItem != null && primaryItem.canUse(this)) {
			return primaryItem.useItem(this);
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
}
