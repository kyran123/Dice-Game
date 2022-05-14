using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ShopManager : MonoBehaviour
{
    public GameObject child;

    public List<BuyButton> buttons;

    public List<ShopItemContainer> itemContainers = new List<ShopItemContainer>();

    public List<DiceContainer> diceContainers = new List<DiceContainer>();

    public List<DiceContainer> diceShopContainers = new List<DiceContainer>();

    void Start()
    {
        BattleManager._instance.OnToggleScreen += this.toggle;
        BattleManager._instance.OnRemoveItem += this.updateShop;
        BattleManager._instance.OnUpdateShop += this.updateShop;
    }


    public void generateDice()
    {
        List<List<int>> die6Sides = BattleManager._instance.GetComponent<DiceManager>().die6Sides;

        foreach(DiceContainer container in diceShopContainers)
        {
            container.addDie(die6Sides[Random.Range(0,die6Sides.Count-1)]);
        }
    }

    public void generateItems()
    {
        BattleManager bm = BattleManager._instance;
        foreach (ShopItemContainer container in this.itemContainers)
        {
            bm.itemManager.getNewShopItem(this.itemContainers);
            container.addItem(bm.itemManager.shopItem);
        }
    }

    public void deleteItems()
    {
        foreach(ShopItemContainer container in this.itemContainers)
        {
            container.removeItem();
        }
    }

    public void updateShop(object sender, eventArgs e)
    {
        foreach(ShopItemContainer container in this.itemContainers)
        {
            container.updateItem();
        }
        List<Die> dice = BattleManager._instance.GetComponent<DiceManager>().dice;
        for(int i = 0; i < dice.Count; i++)
        {
            diceContainers[i].addDie(dice[i]);
        }
        foreach(BuyButton button in this.buttons)
        {
            button.updateShop();
        }
    }

    public void toggle(object sender, eventArgs e)
    {
        if (e.screenValue == screen.Shop)
        {
            child.SetActive(true);
            this.generateItems();
            this.generateDice();
            this.updateShop(this, new eventArgs {});
        }
        else
        {
            child.SetActive(false);
        }
    }
}
