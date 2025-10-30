using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : UnlockableBase
{
    //public ItemData Item;
    public Image icon;
    public int Index;
    //public int Quantity;

    [SerializeField] TextMeshProUGUI _itemName;
    [SerializeField] TextMeshProUGUI _itemStat;
    [SerializeField] TextMeshProUGUI _itemPrice;


    public void Set()
    {
        // _itemName.text = Item.ItemName;
        // _itemPrice.text = Item.Price.ToString();
        // icon.sprite = Item.ItemIcon;

        // switch (Item.Type)
        // {
        //     case ItemType.Mouse:
        //         _itemStat.text = Item.Mental.ToString();
        //         break;
        //     case ItemType.Keyboard:
        //         _itemStat.text = Item.Physical.ToString();
        //         break;
        //     case ItemType.Monitor:
        //         _itemStat.text = Item.HealthReduceRate.ToString();
        //         break;
        // }
        //
        // icon.sprite = Item.ItemIcon;
        //
        // base.SetData(Item);
    }

    
    public void Clear()
    {
        //Item = null;
        //Quantity = 0;
        icon.gameObject.SetActive(false);
    }


    public void OnclickButton()
    {
        // ItemManager.Instance.SelectItem(Index);
        // UIManager.Instance.UI_ShopPopup.RefreshUI();
    }
}