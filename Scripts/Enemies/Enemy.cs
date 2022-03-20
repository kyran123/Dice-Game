using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    private EnemyDisplay display;

    [SerializeField]
    public string Name;
    public int HP;
    public int damage;
    public int minRoll;


    void Start() {
        this.display = this.GetComponent<EnemyDisplay>();
        BattleManager._instance.OnEnemyDamage += this.ModifyHP;
        BattleManager._instance.OnRoll += this.checkMinRoll;
    }

    // positive -> healing , negative -> damage
    public void ModifyHP(object sender, eventArgs e) {
        BattleManager bm = sender as BattleManager;
        HP += e.damage;
        this.display.updateDisplay();
        if(HP <= 0 ) {
            //die
            bm.enemyDeath(this);
        }
    }

    public void checkMinRoll(object sender, eventArgs e)
    {   
        BattleManager bm = sender as BattleManager;
        if(e.roll >= this.minRoll)
        bm.attackEnemy(-this.damage);
        else bm.ModifyPlayerHP(-bm.player.damage);
    }
}