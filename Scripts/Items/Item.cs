using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;


/*
    PASSIVE ITEMS
    Rolls: 1 - 12
    Enemy & Player HP
    More or less than x damage
    Passive (always active, like +1 HP or +1 Damage)
    More or less than x amount of gold
    Double the rewards
    
    ACTIVE ITEMS (Activated on click)
    + to rolls
    Heal
    Do damage
*/
// you receive 1 damage -> change in HP -> Event fired
// Event gets received by all items owned by player
//
//

public class Item : MonoBehaviour, IPointerDownHandler
{
    void Start() {
        BattleManager._instance.OnRoll += this.rollHeal;
        BattleManager._instance.OnPlayerDamage += this.brokenPiggybank;
        BattleManager._instance.OnEnemyDeath += this.brokenPiggybankReward;
    }

    public bool isPassive; // 0 or false active / 1 or true passive
    public Items type; //item name

    public void OnPointerDown(PointerEventData pointer)
    {
        if(this.transform.parent.name == "NewItem") {
            //Add item to player
            BattleManager._instance.addItemToHand(this);
            BattleManager._instance.rewardAdded();
        } else {
            doDamage();
            plusRolls();
            coinHeal();
        }
    }

    #region Passive Items

    //On roll >= 5, heal 1
    public void rollHeal(object sender,eventArgs e)
    {
        if(!isPassive || this.type != Items.rollHeal) return;        
        BattleManager bm = sender as BattleManager;
        if(e.roll>=5) bm.ModifyPlayerHP(1);
    }

    //On all rolls 6 get rolls*10 coins
    public void rollCoin(object sender, eventArgs e)
    {
        if(!isPassive || this.type != Items.rollCoin) return;  
        BattleManager bm = sender as BattleManager;
        if(e.individualRolls.Count() == 1) return;
        int res = e.individualRolls.Sum(roll => 6);
        if(res == e.individualRolls.Count()) {
            bm.giveReward(new eventArgs {coins = e.individualRolls.Count()*10});
        }
    }

    //2x coin rewards from battles, -2 coins on player damage
    public void brokenPiggybank(object sender, eventArgs e)
    {
        if(!isPassive|| this.type != Items.brokenPiggybank) return;
        BattleManager bm = sender as BattleManager;
        bm.modifyCoins(-2);
    }

    public void brokenPiggybankReward(object sender, eventArgs e)
    {
        if(!isPassive || this.type != Items.brokenPiggybank) return;
        Enemy en = sender as Enemy;
        en.gameObject.GetComponent<Reward>().coins *= 2;
    } 

    #endregion

    #region Active Items

    public void doDamage()
    {
        if(isPassive || this.type != Items.doDamage) return;
        BattleManager._instance.attackEnemy(-1);
        Destroy(this.gameObject);
    }

    //on use +1 to dice rolls, capped at 6
    public void plusRolls()
    {
        if(isPassive || this.type != Items.plusRolls) return;
        BattleManager._instance.plusRolls = 1;
        Destroy(this.gameObject);
    }

    //heal 1 hp for 3 coins
    public void coinHeal()
    {
        if(isPassive || this.type != Items.coinHeal) return;
        if(BattleManager._instance.player.coins>=3)
        {
            BattleManager._instance.modifyCoins(-3);
            BattleManager._instance.ModifyPlayerHP(1);
        }
    }

    #endregion
}

public enum Items {
    empty,
    rollHeal, //heal 1 when you roll 5
    doDamage, //do 1 damage
    rollCoin,//On all rolls 6 get rolls*10 coins - min 2 die
    plusRolls, //add +1 to individual rolls, capped at 6
    brokenPiggybank, //2x coin rewards from battles, -2 coins on player damage
    coinHeal, //heal 1 hp for 3 coins
}