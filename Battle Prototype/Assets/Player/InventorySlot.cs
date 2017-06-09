using UnityEngine;
using System.Collections;

public class InventorySlot : MonoBehaviour 
{
	public InventoryItem primaryItem;
    public VariableStore variableStore;

    public int GetAmmoCount(AmmoType type)
    {
        return variableStore.GetInt(type.ToString() + "_count");
    }

    public void GiveAmmo(AmmoType type, int amount)
    {
        variableStore.SetInt(type.ToString() + "_count", GetAmmoCount(type) + amount);
    }

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
