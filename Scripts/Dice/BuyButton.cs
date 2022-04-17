using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuyButton : MonoBehaviour, IPointerDownHandler
{
    public bool canbuy;

    public void updateShop()
    {
        DiceContainer container = this.transform.GetComponentInParent<DiceContainer>();
        if (container.displaySides.Count > 1 || container.currentDie != null)
        {
            if (BattleManager._instance.player.coins >= container.diePrice && BattleManager._instance.GetComponent<DiceManager>().canBuyDie())
            {
                this.canbuy = true;
                container.price.color = Color.black;
            }
            else
            {
                this.canbuy = false;
                container.price.color = Color.red;
            }
        }
    }

    public void OnPointerDown(PointerEventData pointer)
    {
        if (this.canbuy)
        {
            DiceContainer container = this.transform.GetComponentInParent<DiceContainer>();
            BattleManager._instance.addDie(container.displaySides);
            container.text.text = "";
            container.price.text = "Sold";
            BattleManager._instance.modifyCoins(-container.diePrice);
            BattleManager._instance.updateShop();
        }
    }
}
