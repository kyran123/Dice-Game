using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ItemManager : MonoBehaviour
{
    public List<ItemContainer> itemContainers = new List<ItemContainer>();

    public List<GameObject> allItems = new List<GameObject>();

    public GameObject text;

    void Start()
    {
        BattleManager._instance.OnAddItemToHand += this.addItemToHand;
        BattleManager._instance.OnRewardAdded += this.resetHandMsg;
        BattleManager._instance.OnRemoveRandomItem += this.removeRandomItem;
        BattleManager._instance.OnAddEventItem += this.getEventItem;
        BattleManager._instance.OnRedrawHand += this.redrawHand;
        BattleManager._instance.OnToggleCurse += this.toggleCurseFlip;
    }

    public void resetHandMsg(object sender, eventArgs e)
    {
        this.text.SetActive(false);
    }

    public GameObject newItemObject;
    public void getNewItem()
    {
        Item item = this.allItems[Random.Range(0, this.allItems.Count - 1)].GetComponent<Item>();
        if (this.itemContainers.Any(i => i.getItem() != null && i.getItem().type == item.type && item.isCurse))
        {
            this.getNewItem();
        }
        else
        {
            this.newItemObject = Instantiate(item.gameObject);
        }
    }

    public Item shopItem;
    public void getNewShopItem(List<ShopItemContainer> shopContainers)
    {
        Item item = this.allItems[Random.Range(0, this.allItems.Count - 1)].GetComponent<Item>();
        if (
            item.isCurse ||
            shopContainers.Any(i => i.item != null && i.item.type == item.type)
        )
        {
            this.getNewShopItem(shopContainers);
        }
        else
        {
            this.shopItem = Instantiate(item.gameObject).GetComponent<Item>();
            this.shopItem.removable = false;
        }
    }

    public void getRandomItem(Enemy enemy)
    {
        this.getNewItem();
        BattleManager._instance.showNewItem(this.newItemObject);
    }

    public void addItemToHand(object sender, eventArgs e)
    {
        if (!this.isHandFull())
        {
            BattleManager._instance.rewardAdded();
            foreach (ItemContainer container in this.itemContainers)
            {
                if (container.getItem() == null)
                {
                    container.addItem(e.itemObject);
                    BattleManager._instance.handIsFull();
                    return;
                }
            }
        }
    }

    public void getEventItem(object sender, eventArgs e)
    {
        if (!this.isHandFull())
        {
            if (e.itemObject != null)
            {
                this.newItemObject = Instantiate(e.itemObject);
            }
            else
            {
                this.getNewItem();
            }
            foreach (ItemContainer container in this.itemContainers)
            {
                if (container.getItem() == null)
                {
                    container.addItem(this.newItemObject);
                    //TODO: Hide event
                    break;
                }
            }
        }
        BattleManager._instance.handIsFull();
    }

    public void redrawHand(object sender, eventArgs e)
    {
        foreach (ItemContainer container in this.itemContainers)
        {
            if (container.item == null) continue;
            container.removeItem();
            this.getNewItem();
            container.addItem(this.newItemObject);
            BattleManager._instance.handIsFull();
        }
    }

    public bool isHandFull(bool external = false)
    {
        foreach (ItemContainer container in this.itemContainers)
        {
            if (container.getItem() == null)
            {
                return false;
            }
        }
        if (!external) this.text.SetActive(true);
        return true;
    }

    public void removeRandomItem(object sender, eventArgs e)
    {
        List<ItemContainer> fullContainers = new List<ItemContainer>();
        foreach (ItemContainer container in this.itemContainers)
        {
            if (container.item != null)
                fullContainers.Add(container);
        }
        if (fullContainers.Count() != 0)
        {
            fullContainers[Random.Range(0, fullContainers.Count())].removeItem();
            BattleManager._instance.handIsFull();
        }
    }

    public void toggleCurseFlip(object sender, eventArgs e)
    {
        foreach (ItemContainer container in itemContainers)
        {
            if (container.getItem() != null) container.getItem().removable = !container.getItem().removable;
        }
    }

}
