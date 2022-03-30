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
    }

    // positive -> healing , negative -> damage
    public void ModifyHP(object sender, eventArgs e) {
        HP += e.damage;
        this.updateDisplay();
        if(HP <= 0 ) {
            //die
            //check for player death
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
        if(e.coins < 0 && coins < Mathf.Abs(e.coins)) return;
        coins += e.coins;
        updateDisplay();
    }

    public void updateDisplay() {
        this.HPText.text = $"{this.HP}";
        this.damageText.text = $"{this.damage}";
        this.coinsText.text = $"{this.coins}";
    }

    public void toggle(object sender, eventArgs e) {
        if(e.screenValue != screen.GameOver) this.gameObject.SetActive(true);
        else this.gameObject.SetActive(false);
    }
}
