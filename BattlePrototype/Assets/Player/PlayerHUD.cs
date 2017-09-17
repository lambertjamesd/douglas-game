using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerHUD : MonoBehaviour {
	public Text textMesh;
	public InventorySlot inventory;
    public Image gunIcon;
    public Damageable playerDamage;
    public RectTransform heatlhBar;
    private Rect fullHeathRect;

    public List<GameObject> namedGUIs = new List<GameObject>();

    public void SetGUIVisible(string name, bool value)
    {
        GameObject gui = namedGUIs.Find((item) => item.name == name);

        if (gui != null)
        {
            gui.SetActive(value);
        }
    }

	// Use this for initialization
	void Start () {
        fullHeathRect = heatlhBar.rect;
		transform.SetParent(null, false);
	}

	void Update () {
        GunStats currentGun = inventory.GetCurrentGun();

        if (currentGun != null && inventory.HasAGun())
        {
            textMesh.gameObject.SetActive(true);
            gunIcon.gameObject.SetActive(true);
            textMesh.text = inventory.GetAmmoCount(currentGun.type).ToString();
            gunIcon.sprite = currentGun.smallIcon;
        }
        else
        {
            textMesh.gameObject.SetActive(false);
            gunIcon.gameObject.SetActive(false);
        }
        heatlhBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, fullHeathRect.width * playerDamage.CurrentHealth / playerDamage.maxHealth);
    }
}
