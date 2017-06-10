using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventorySlot : MonoBehaviour 
{
	public InventoryItem primaryItem;
    public VariableStore variableStore;
    public Arsenal usableWeapons;
    public int currentGunIndex = -1;

    public GunStats GetCurrentGun()
    {
        if (currentGunIndex == -1)
        {
            return null;
        }
        else
        {
            return usableWeapons.guns[currentGunIndex];
        }
    }

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
