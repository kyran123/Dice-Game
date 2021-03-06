using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[System.Serializable]
public class Reward
{
    [SerializeField]
    public int HP;
    public int coins;
    public int playerDamage;
    public itemReward item;
    public GameObject specificItem;
    public dieReward die;

    public bool validate(BattleManager bm)
    {
        bool flag = false;
        if(HP != 0) 
        {
            flag = true;
        }
        if (coins < 0)
        {
            if (bm.player.coins >= Mathf.Abs(coins)) flag = true;
            else return false;
        }
        if (playerDamage < 0)
        {
            if (bm.player.damage > Mathf.Abs(playerDamage)) flag = true;
            else return false;
        }
        if (die == dieReward.RemoveRandom)
        {
            if (bm.GetComponent<DiceManager>().canRemovedie()) flag = true;
            else return false;
        }
        if(die == dieReward.AddRandom)
        {
            if(bm.GetComponent<DiceManager>().canBuyDie()) flag = true;
            else return false;
        }
        if (item == itemReward.AddRandom)
        {
            if (!bm.itemManager.isHandFull(true)) flag = true;
            else return false;
        }
        return flag;
    }

    public void addReward()
    {
        BattleManager bm = BattleManager._instance;

        if (HP != 0)
        {
            bm.ModifyPlayerHP(HP);
        }
        if (coins != 0)
        {
            bm.modifyCoins(coins);
        }
        if (playerDamage != 0)
        {
            bm.modifyPlayerDamage(playerDamage);
        }
        if (item != itemReward.Nothing)
        {
            switch (item)
            {
                case itemReward.RemoveRandom:
                    bm.removeRandomItem();
                    break;
                case itemReward.RemoveRandomNonCurse:
                    bm.removeRandomNonCurseItem();
                    break;
                case itemReward.AddRandom:
                    bm.addEventItem(specificItem);
                    break;
                case itemReward.AddRandomNonCurse:
                    bm.addNonCurseEventItem();
                    break;
                case itemReward.AddCurse:
                    bm.addCurseItem();
                    break;
                case itemReward.RedrawHand:
                    bm.redrawHand();
                    break;
            }
        }
        if (die != dieReward.Nothing)
        {
            switch (die)
            {
                case dieReward.RemoveRandom:
                    bm.removeDie(null);
                    break;
                case dieReward.AddRandom:
                    bm.addDie(new List<int>());
                    break;
            }
        }

    }
}

public enum itemReward
{
    Nothing,
    RemoveRandom,
    RemoveRandomNonCurse,
    AddRandom,
    AddRandomNonCurse,
    AddCurse,
    RedrawHand
}

public enum dieReward
{
    Nothing,
    RemoveRandom,
    AddRandom
}