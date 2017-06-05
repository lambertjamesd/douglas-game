using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public class PlayerInventory : InventorySlot
{
    private string NameForAmmo(AmmoType type)
    {
        return type.ToString() + "_count";
    }

    public override int GetAmmoCount(AmmoType type)
    {
        object result = StoryManager.GetSingleton().GetStory().variablesState[NameForAmmo(type)];

        if (result is int)
        {
            return (int)result;
        }
        else if (result == null)
        {
            return 0;
        }
        else
        {
            throw new System.Exception(NameForAmmo(type) + " is not a number");
        }
    }
    public override void GiveAmmo(AmmoType type, int amount)
    {
        StoryManager.GetSingleton().GetStory().variablesState[NameForAmmo(type)] = GetAmmoCount(type) + amount;
    }
}
