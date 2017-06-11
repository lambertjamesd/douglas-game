using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

    public void GiveGun(string name)
    {
        variableStore.SetBool("has_" + name, true);

        if (currentGunIndex == -1)
        {
            currentGunIndex = usableWeapons.guns.FindIndex((item) => item.gunName == name);
        }
    }

    public bool HasGun(string name)
    {
        return variableStore.GetBool("has_" + name);
    }

    public void NextGun()
    {
        for (int i = 1; i <= usableWeapons.guns.Count; ++i)
        {
            int finalIndex = (currentGunIndex + i) % usableWeapons.guns.Count;

            if (HasGun(usableWeapons.guns[finalIndex].gunName))
            {
                currentGunIndex = finalIndex;
                break;
            }
        }
    }

    public void PrevGun()
    {
        for (int i = 1; i <= usableWeapons.guns.Count; ++i)
        {
            int finalIndex = (usableWeapons.guns.Count * 2 + currentGunIndex - i) % usableWeapons.guns.Count;

            if (HasGun(usableWeapons.guns[finalIndex].gunName))
            {
                currentGunIndex = finalIndex;
                break;
            }
        }
    }

    public GunStats[] GetAvailableGuns()
    {
        return usableWeapons.guns.Where((gun) => HasGun(gun.gunName)).ToArray();
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
