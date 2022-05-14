using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour {
    
    public int HP;
    public TMP_Text HPText;
    public int damage;
    public TMP_Text damageText;
    public int coins = 0;
    public TMP_Text coinsText;

    void Start() {
        updateDisplay();
        BattleManager._instance.OnPlayerDamage += this.ModifyHP;
        BattleManager._instance.OnReward += this.ModifyCoins;
        BattleManager._instance.OnModifyCoins += this.ModifyCoins;
        BattleManager._instance.OnToggleScreen += this.toggle;
        BattleManager._instance.OnModifyPlayerDamage += this.ModifyPlayerDamage;
        
        //Debug
        BattleManager._instance.debugSetPlayerHP += this.setHP;
        BattleManager._instance.debugSetCoins += this.setCoins;
        BattleManager._instance.debugSetPlayerDamage += this.setDamage;
    }

    public void setHP(object sender, eventArgs e)
    {
        this.HP = e.debug_int;
        this.updateDisplay();
        if(HP <= 0 ) {
            this.death(this, new eventArgs { });
        }
    }

    public int getDamage() {
        return this.damage + BattleManager._instance.itemManager.getItemsValue(Items.BigStick);
    }

    public void setDamage(object sender, eventArgs e)
    {
        this.damage = e.debug_int;
        this.updateDisplay();
    }

    // positive -> healing , negative -> damage
    public void ModifyHP(object sender, eventArgs e) {
        HP += e.damage;
        this.updateDisplay();
        if(HP <= 0 ) {
            this.death(this, new eventArgs { });
        }
    }

    public void setCoins(object sender, eventArgs e)
    {
        this.coins = e.debug_int;
        updateDisplay();
    }

    public void death(object sender, eventArgs e)
    {
        if(BattleManager._instance.itemManager.hasItem(Items.CyclingHelmet))
        {
            HP = 1;
            BattleManager._instance.removeCyclingHelmet();
            updateDisplay();
        }
        else
        {
            BattleManager._instance.playerDeath();
        }
    }

    public void ModifyPlayerDamage(object sender, eventArgs e)
    {
        if(damage + e.damage <= 0) damage = 1;
        else damage += e.damage;
        this.updateDisplay();
    }

    // positive -> add / negative -> subtract
    public void ModifyCoins(object sender, eventArgs e) {
        coins += e.coins;
        if(coins < 0) coins = 0;
        updateDisplay();
    }

    public void updateDisplay() {
        this.HPText.text = $"{this.HP}";
        this.damageText.text = $"{this.getDamage()}";
        this.coinsText.text = $"{this.coins}";
    }

    public void toggle(object sender, eventArgs e) {
        if(e.screenValue != screen.GameOver) this.gameObject.SetActive(true);
        else this.gameObject.SetActive(false);
    }
}
