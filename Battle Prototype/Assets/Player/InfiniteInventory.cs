using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteInventory : InventorySlot {
    public override int GetAmmoCount(AmmoType type)
    {
        return 10;
    }
    public override void GiveAmmo(AmmoType type, int amount)
    {
        // intentially blank
    }
}
