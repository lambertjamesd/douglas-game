using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHUD : MonoBehaviour {
	public Text textMesh;
	public InventorySlot inventory;
    public Image gunIcon;

	// Use this for initialization
	void Start () {
		transform.SetParent(null, false);
	}

	void Update () {
        GunStats currentGun = inventory.GetCurrentGun();

        if (currentGun != null)
        {
            textMesh.text = inventory.GetAmmoCount(currentGun.type).ToString();
            gunIcon.sprite = currentGun.smallIcon;
        }

    }
}
