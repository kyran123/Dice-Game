using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


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

public enum ItemType {
    Passive,
    Active
}

public class Item : MonoBehaviour, IPointerDownHandler
{
    void Start() {
        BattleManager._instance.OnRoll += this.rollHeal;
        BattleManager._instance.OnPlayerDamage += this.brokenPiggybank;
        BattleManager._instance.OnEnemyDeath += this.brokenPiggybankReward;
        BattleManager._instance.OnBattle += this.resetUsed;
        this.Description.text = this.generateDescription();
    }

    public void resetUsed(object sender, eventArgs e) {
        this.used = false;
        this.transform.localPosition = Vector3.zero;
    }

    [Tooltip("0 for active, 1 for passive")]
    public ItemType itemType;
    [Tooltip("If you can use the item only once per battle")]
    public bool oncePerBattle; //If you can use the card only once per battle
    private bool used;
    [Tooltip("If the item gets destroyed on use")]
    public bool destroyOnUse; //If the item gets destroyed on use

    public Items type; //item name

    [Header("TMPro")]
    public TMP_Text title;
    public TMP_Text Description;

    public void OnPointerDown(PointerEventData pointer)
    {
        if(pointer.button == PointerEventData.InputButton.Right) {
            Destroy(this.gameObject);
        }
        if(pointer.button == PointerEventData.InputButton.Left) {
            if(this.transform.parent.name == "NewItem") {
                //Add item to player
                BattleManager._instance.addItemToHand(this);
            } else {
                doDamage();
                plusRolls();
                coinHeal();
            }
        }
    }

    public bool validate(ItemType iType, Items t) {
        if(this.itemType != iType) return false;
        if(this.type != t) return false;
        if(this.oncePerBattle && this.used) return false;
        return true;
    }

    #region Passive Items

    //On roll >= 5, heal 1
    public void rollHeal(object sender,eventArgs e)
    {
        if(!this.validate(ItemType.Passive, Items.rollHeal)) return;  
        BattleManager bm = sender as BattleManager;
        this.used = true;
        if(e.roll>=5) bm.ModifyPlayerHP(1);
    }

    //On all rolls 6 get rolls*10 coins
    public void rollCoin(object sender, eventArgs e)
    {
        if(!this.validate(ItemType.Passive, Items.rollCoin)) return;
        BattleManager bm = sender as BattleManager;
        if(e.individualRolls.Count() == 1) return;
        int res = e.individualRolls.Sum(roll => 6);
        if(res == e.individualRolls.Count()) {
            this.used = true;
            bm.giveReward(new eventArgs {coins = e.individualRolls.Count()*10});
        }
    }

    //2x coin rewards from battles, -2 coins on player damage
    public void brokenPiggybank(object sender, eventArgs e)
    {
        if(!this.validate(ItemType.Passive, Items.brokenPiggybank)) return;
        BattleManager bm = sender as BattleManager;
        if(e.damage > 0) return;
        this.used = true;
        bm.modifyCoins(-2);
    }

    public void brokenPiggybankReward(object sender, eventArgs e)
    {
        if(!this.validate(ItemType.Passive, Items.brokenPiggybank)) return;
        Enemy en = sender as Enemy;
        this.used = true;
        en.gameObject.GetComponent<Reward>().coins *= 2;
    } 

    #endregion

    #region Active Items

    //Deal 1 damage
    public void doDamage()
    {
        if(!this.validate(ItemType.Active, Items.doDamage)) return;
        this.used = true;
        if(this.oncePerBattle) this.transform.localPosition = new Vector3(0f, -5f, 0f);
        BattleManager._instance.attackEnemy(-1);
        Destroy(this.gameObject);
    }

    //on use +1 to dice rolls, capped at 6
    public void plusRolls()
    {
        if(!this.validate(ItemType.Active, Items.plusRolls)) return;
        this.used = true;
        if(this.oncePerBattle) this.transform.localPosition = new Vector3(0f, -5f, 0f);
        BattleManager._instance.plusRolls = 1;
        Destroy(this.gameObject);
    }

    //heal 1 hp for 3 coins
    public void coinHeal()
    {
        if(!this.validate(ItemType.Active, Items.coinHeal)) return;
        if(BattleManager._instance.player.coins>=3)
        {
            this.used = true;
            if(this.oncePerBattle) this.transform.localPosition = new Vector3(0f, -5f, 0f);
            BattleManager._instance.modifyCoins(-3);
            BattleManager._instance.ModifyPlayerHP(1);
        }
    }

    #endregion

    public string generateDescription() {
        //Generate string for description here
        switch(this.type) {
            case Items.brokenPiggybank:
                this.title.text = "Broken piggybank";
                return $"{this.getConditional()}Lose 2 coins on damage. Gain double the rewards of battle.";
            case Items.rollCoin:
                this.title.text = "Roll Coin";
                return $"{this.getConditional()}Get 10 coins per dice every time all of them roll 6 (Minimum 2 die).";
            case Items.rollHeal:
                this.title.text = "Roll Heal";
                return $"{this.getConditional()}Heal 1 HP whenever you roll at least 5.";
            case Items.doDamage:
                this.title.text = "Do Damage";
                return $"{this.getConditional()}Do 1 damage to the enemy.";
            case Items.plusRolls:
                this.title.text = "Plus Rolls";
                return $"{this.getConditional()}Add +1 to all individual rolls (max 6).";
            case Items.coinHeal:
                this.title.text = "Coin Heal";
                return $"{this.getConditional()}Spend 3 coins to heal 1 HP.";
            default:
                return "";
        }
    }

    public string getConditional() {
        if(this.destroyOnUse) return "Destroy this. ";
        if(this.oncePerBattle) return "Once per battle. ";
        return "";
    }
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