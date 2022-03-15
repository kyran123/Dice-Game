using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    public GameObject item;

    public void addItem(GameObject itemGO) {
        itemGO.transform.SetParent(this.transform);
        this.item = itemGO;
        this.item.transform.localPosition = new Vector3(0f, 0f, 0f);
    }

    public Item getItem() {
        if(this.item != null) return this.item.GetComponent<Item>();
        else return null;
    }

    public void removeItem() {
        Destroy(this.item);
        this.item = null;
    }
}
