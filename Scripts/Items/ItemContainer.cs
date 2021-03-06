using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    public GameObject item;

    public void addItem(GameObject itemGO)
    {
        this.item = itemGO;
        this.item.GetComponent<Item>().subscribe();
        this.item.transform.SetParent(this.transform, false);
    }

    public Item getItem()
    {
        if (this.item != null) return this.item.GetComponent<Item>();
        else return null;
    }

    public void removeItem()
    {
        this.item.GetComponent<Item>().unSubscribe();
        Destroy(this.item);
        this.item = null;
        BattleManager._instance.handIsFull();
    }
}
