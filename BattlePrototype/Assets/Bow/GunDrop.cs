using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunDrop : MonoBehaviour {
    public GunStats gun;

    public void OnTriggerEnter2D(Collider2D collider)
    {
        InventorySlot inventory = collider.GetComponent<InventorySlot>();

        if (inventory != null)
        {
            inventory.GiveGun(gun.gunName);
            Destroy(gameObject);
        }
    }
}
