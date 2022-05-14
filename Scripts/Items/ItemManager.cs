using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ItemManager : MonoBehaviour
{
    public List<ItemContainer> itemContainers = new List<ItemContainer>();

    public List<GameObject> allItems = new List<GameObject>();
    public List<GameObject> potentialItems = new List<GameObject>();

    public GameObject text;

    void Start()
    {
        BattleManager._instance.OnAddItemToHand += this.addItemToHand;
        BattleManager._instance.OnRewardAdded += this.resetHandMsg;
        BattleManager._instance.OnRemoveRandomItem += this.removeRandomItem;
        BattleManager._instance.OnAddEventItem += this.getEventItem;
        BattleManager._instance.OnRedrawHand += this.redrawHand;
        BattleManager._instance.OnToggleCurse += this.toggleCurseFlip;
        BattleManager._instance.OnAddcurseItem += this.getCurseItem;

        BattleManager._instance.debugAddItem += this.debug_getItem;
    }

    public void resetHandMsg(object sender, eventArgs e)
    {
        this.text.SetActive(false);
    }

    public GameObject newItemObject;
    public void getNewItem()
    {
        if(this.potentialItems.Count == 0) return;
        Item item = this.potentialItems[Random.Range(0, this.potentialItems.Count)].GetComponent<Item>();
        this.potentialItems.Remove(item.gameObject);
        if (this.itemContainers.Any(i => i.getItem() != null && (i.getItem().type == item.type)))
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
        List<GameObject> potentialItems = this.allItems.Where(i => i.GetComponent<Item>().isCurse == false).ToList();
        Item item = potentialItems[Random.Range(0, potentialItems.Count)].GetComponent<Item>();
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

    public void getRandomItem()
    {
        this.potentialItems = this.allItems.Where(i => i.GetComponent<Item>().isCurse == false).ToList();
        this.getNewItem();
        BattleManager._instance.showNewItem(this.newItemObject);
    }

    public void getCurseItem(object sender, eventArgs e)
    {
        if(!this.isHandFull())
        {
            this.potentialItems = this.allItems.Where(i => i.GetComponent<Item>().isCurse == true).ToList();
            this.getNewItem();
            this.addItemToHand(this, new eventArgs { itemObject = this.newItemObject });
        }
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
                    BattleManager._instance.player.updateDisplay();
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
                if(e.startEvent) this.potentialItems = this.allItems.Where(i => i.GetComponent<Item>().isCurse == false).ToList();
                else this.potentialItems = this.allItems.ToList();
                this.getNewItem();
            }
            foreach (ItemContainer container in this.itemContainers)
            {
                if (container.getItem() == null)
                {
                    container.addItem(this.newItemObject);
                    break;
                }
            }
        }
        BattleManager._instance.handIsFull();
    }

    public void debug_getItem(object sender, eventArgs e)
    {
        if(!this.isHandFull())
        {
            List<GameObject> items = this.allItems.Where(i => BattleManager._instance.compareStrings(i.GetComponent<Item>().type.ToString(), e.debug_string)).ToList();
            if(items.Count > 0) 
            {
                this.addItemToHand(this, new eventArgs { itemObject = Instantiate(items[0]) });
            }
            else BattleManager._instance.message("Item not found");
        }
        else 
        {
            BattleManager._instance.message("Hand is full");
        }
    }

    public void redrawHand(object sender, eventArgs e)
    {
        foreach (ItemContainer container in this.itemContainers)
        {
            if (container.item == null) continue;
            container.removeItem();
            this.potentialItems = this.allItems.ToList();
            this.getNewItem();
            container.addItem(this.newItemObject);
            BattleManager._instance.handIsFull();
        }
    }

    public int itemCount()
    {
        int count = 0;
        foreach (ItemContainer container in this.itemContainers)
        {
            if (container.getItem() != null)
            {
                count++;
            }
        }
        return count;
    }

    public int getItemsValue(Items type)
    {
        int totalValue = 0;
        foreach(ItemContainer container in this.itemContainers)
        {
            if(container.getItem() != null)
            {
                if(container.getItem().type == type) totalValue += container.getItem().value;
            }
        }
        return totalValue;
    }

    public bool hasItem(Items type) 
    {
        foreach(ItemContainer container in this.itemContainers)
        {
            if(container.item != null)
            {
                if(container.getItem().type == type) return true;
            }
        }
        return false;
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

    public List<Item> getAllItems()
    {
        List<Item> items = new List<Item>();
        foreach (ItemContainer container in this.itemContainers)
        {
            if (container.getItem() != null)
            {
                items.Add(container.getItem());
            }
        }
        return items;
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
            if(e.item) 
            {
                fullContainers = fullContainers.Where(con => con.getItem().isCurse == false).ToList();
            }
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
