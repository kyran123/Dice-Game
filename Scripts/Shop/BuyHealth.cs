using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuyHealth : MonoBehaviour, IPointerDownHandler
{
    int hp = 2;
    int cost = 10;

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        if(BattleManager._instance.player.coins >= cost)
        {
            BattleManager._instance.ModifyPlayerHP(hp);
            BattleManager._instance.modifyCoins(-cost);
        }
    }
}
