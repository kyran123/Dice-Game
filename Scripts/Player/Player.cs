using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour {
    
    public int HP;
    public TMP_Text HPText;
    public int damage;
    public TMP_Text damageText;

    void Start() {
        BattleManager._instance.OnPlayerDamage += this.ModifyHP;
    }

    // positive -> healing , negative -> damage
    public void ModifyHP(object sender, eventArgs e)
    {
        HP += e.damage;
        this.updateDisplay();
        if(HP<=0)
        {
            //die
            //check for player death
        }
    }

    public void updateDisplay() {
        this.HPText.text = $"{this.HP}";
        this.damageText.text = $"{this.damage}";
    }

}
