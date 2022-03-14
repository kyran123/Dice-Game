using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;


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

public class Item : MonoBehaviour
{
    void Start() {
        BattleManager._instance.OnRoll += this.rollHeal;
    }

    public bool itemType; // 0 or false active / 1 or true passive
    public Items type; //item name

    void OnMouseDown()
    {
        doDamage();
    }

    #region Passive Items

    //On roll >= 5, heal 1
    public void rollHeal(object sender,eventArgs e)
    {
        if(!itemType || this.type != Items.rollHeal) return;        
        BattleManager bm = sender as BattleManager;
        if(e.roll>=5) bm.ModifyPlayerHP(1);
    }

    #endregion

    #region Active Items

    public void doDamage()
    {
        if(itemType || this.type != Items.doDamage) return;
        BattleManager._instance.attackEnemy(-1);
        Destroy(this.gameObject);
    }

    #endregion
}

public enum Items {
    rollHeal, //heal 1 when you roll 5
    doDamage //do 1 damage
}