using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class ItemManager : MonoBehaviour
{
    public List<ItemContainer> itemContainers = new List<ItemContainer>();

    public List<GameObject> allItems = new List<GameObject>();

    void Start() {
        BattleManager._instance.OnReward += this.addItem;
    }

    public void addItem(object sender, eventArgs e) {
        if(!e.item) return;
        this.getNewItem();
    }

    public void getNewItem() {
        Item item = this.allItems[UnityEngine.Random.Range(0, this.allItems.Count - 1)].GetComponent<Item>();
        if(this.itemContainers.Any(i => i.getItem() != null && i.getItem().type == item.type)) {
            this.getNewItem();
        } else {
            foreach(ItemContainer container in this.itemContainers) {
                Item i = container.getItem();
                if(i == null) {
                    GameObject itemGO = Instantiate(item.gameObject);
                    container.addItem(itemGO);
                    return;
                }
            }
        }
    }

    public void removeItem() {

    }
}
