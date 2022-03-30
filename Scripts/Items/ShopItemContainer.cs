using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopItemContainer : MonoBehaviour
{
    public TMP_Text text;
    public Item item;


    public void addItem(Item item)
    {
        this.item = item;
        this.item.transform.SetParent(this.transform, false);
        this.text.text = $"{this.item.shopValue.ToString()} coins";
        if (!this.item.canBuy()) text.color = Color.red;
    }

    public void updateItem()
    {
        if(item != null)
        {
            if (!this.item.canBuy()) text.color = Color.red;
            else text.color = Color.black;
        }
    }

    public void reset()
    {
        this.text.text = "";
        this.text.color = Color.black;
    }
}
