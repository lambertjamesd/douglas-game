using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StoreGUIRow : MonoBehaviour, UnityEngine.EventSystems.IPointerEnterHandler
{
    public Button button;
    public Text itemName;
    public Text price;
    public RectTransform rectTransform;

    public delegate void OnEnter();

    public List<OnEnter> onEnterEvents = new List<OnEnter>();

    public void OnPointerEnter(PointerEventData eventData)
    {
        onEnterEvents.ForEach(callback => callback());
    }

    public void UseItem(StoreItem item)
    {
        itemName.text = item.displayName;
        price.text = "$" + item.cost;
        button.interactable = item.isEnabled();
    }
}
