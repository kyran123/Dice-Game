using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class ItemManager : MonoBehaviour
{
    public List<ItemContainer> itemContainers = new List<ItemContainer>();

    public List<GameObject> allItems = new List<GameObject>();

    public GameObject text;

    void Start() {
        BattleManager._instance.OnEnemyDeath += this.getRandomItem;
        BattleManager._instance.OnAddItemToHand += this.addItemToHand;
        BattleManager._instance.OnRewardAdded += this.resetHandMsg;
    }

    public void resetHandMsg(object sender, eventArgs e) {
        this.text.SetActive(false);
    }

    public GameObject newItemObject;
    public void getNewItem() {
        Item item = this.allItems[UnityEngine.Random.Range(0, this.allItems.Count - 1)].GetComponent<Item>();
        if(this.itemContainers.Any(i => i.getItem() != null && i.getItem().type == item.type)) {
            this.getNewItem();
        } else {
            this.newItemObject = item.gameObject;
        }
    }

    public void getRandomItem(object sender, eventArgs e) {
        Enemy enemy = sender as Enemy;
        if(!enemy.GetComponent<Reward>().item) return;
        this.getNewItem();
        BattleManager._instance.showNewItem(this.newItemObject);
    }

    public void addItemToHand(object sender, eventArgs e) {
        if(!this.isHandFull()) {
            BattleManager._instance.rewardAdded();
            foreach(ItemContainer container in this.itemContainers) {
                if(container.getItem() == null) {
                    container.addItem(e.itemObject);
                    return;
                }
            }
        } else {
            //Hand is full and we need to select which card to discard
            Debug.Log("Hand is full!");
            this.text.SetActive(true);
        }
    }

    public bool isHandFull() {
        foreach(ItemContainer container in this.itemContainers) {
            if(container.getItem() == null) {
                return false;
            }
        }
        return true;
    }

    public void removeItem() {

    }
}
