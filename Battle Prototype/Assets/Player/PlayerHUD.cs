using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHUD : MonoBehaviour {
	public Text textMesh;
	public InventorySlot inventory;

	// Use this for initialization
	void Start () {
		transform.SetParent(null, false);
	}

	void Update () {
		textMesh.text = inventory.inventory.arrows.ToString();
	}
}
