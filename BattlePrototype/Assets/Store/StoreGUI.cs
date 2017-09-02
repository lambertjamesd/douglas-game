using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreGUI : MonoBehaviour
{
    public StoreInventory[] stores;

    public Text description;
    public Text playerMoney;
    public Image imageIcon;

    public RectTransform itemStart;

    public StoreGUIRow storeItemPrefab;


    private StoreInventory currentStore;
    private List<StoreGUIRow> activeItems = new List<StoreGUIRow>();

    private void Start()
    {
        UseStore(null);
        TimePause.ScaleTime(0.0f);
    }

    public void DescribeItem(StoreItem item)
    {
        imageIcon.sprite = item.visual;
        description.text = item.description;
    }

    public void BuyItem(StoreItem item)
    {
        item.Buy();
        UseStore(currentStore.name);
    }

    public void ExitStore()
    {
        Destroy(gameObject);
        TimePause.UnscaleTime(0.0f);
    }

    public void UseStore(string name)
    {
        playerMoney.text = "$" + StoryManager.GetSingleton().GetInt("player_money").ToString();

        currentStore = stores.Length > 0 ? stores[0] : null;

        for (int i = 0; i < stores.Length; ++i)
        {
            if (stores[i].name == name)
            {
                currentStore = stores[i];
                break;
            }
        }

        activeItems.ForEach((item) => Destroy(item.gameObject));
        activeItems.Clear();

        Selectable prevItem = null;

        if (currentStore != null)
        {
            for (int i = 0; i < currentStore.inventory.Length; ++i)
            {
                StoreItem item = currentStore.inventory[i];
                StoreGUIRow row = Instantiate(storeItemPrefab);

                row.UseItem(item);
                row.rectTransform.position = itemStart.position;
                row.rectTransform.Translate(new Vector3(0.0f, -itemStart.rect.height * i, 0.0f));
                row.rectTransform.parent = itemStart.parent;
                row.rectTransform.localScale = Vector3.one;
                Navigation buttonNav = row.button.navigation;
                buttonNav.selectOnUp = prevItem;
                row.button.navigation = buttonNav;

                if (prevItem != null)
                {
                    Navigation prevNav = prevItem.navigation;
                    prevNav.selectOnDown = row.button;
                    prevItem.navigation = prevNav;
                }

                row.onEnterEvents.Add(() => DescribeItem(item));
                row.button.onClick.AddListener(() => BuyItem(item));

                activeItems.Add(row);
            }
        }
    }
}
