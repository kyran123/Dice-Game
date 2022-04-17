using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    private EnemyDisplay display;

    [SerializeField]
    public string Name;
    public int HP;
    public int damage;
    public int minRoll;
    [Range(1, 4), Tooltip("1 -> Easy, 2 -> Medium, 3 -> Hard, 4 -> Elite")]
    public int difficultyValue;

    public int coinReward;
    public bool itemReward;

    public List<GameObject> allEnemies = new List<GameObject>();

    void Start()
    {
        this.display = this.GetComponent<EnemyDisplay>();
        BattleManager._instance.OnEnemyDamage += this.ModifyHP;
        BattleManager._instance.OnRoll += this.checkMinRoll;
        BattleManager._instance.OnModifyEnemyDamage += this.ModifyDamage;
        BattleManager._instance.OnModifyMinRolls += this.ModifyMinRoll;
    }

    // positive -> healing , negative -> damage
    public void ModifyHP(object sender, eventArgs e)
    {
        BattleManager bm = sender as BattleManager;
        HP += e.damage;
        this.display.updateDisplay();
        if (HP <= 0)
        {
            //die
            if(coinReward != 0) bm.modifyCoins(coinReward);
            this.GetComponent<EnemySkills>().unSubscribe();
            this.unSubscribe();
            bm.enemyDeath(this);
        }
    }

    public void unSubscribe() {
        BattleManager._instance.OnEnemyDamage -= this.ModifyHP;
        BattleManager._instance.OnRoll -= this.checkMinRoll;
        BattleManager._instance.OnModifyEnemyDamage -= this.ModifyDamage;
        BattleManager._instance.OnModifyMinRolls -= this.ModifyMinRoll;
    }

    public void ModifyDamage(object sender, eventArgs e)
    {
        this.damage += e.damage;
        this.display.updateDisplay();
    }

    public void ModifyMinRoll(object sender, eventArgs e)
    {
        this.minRoll += e.roll;
        this.display.updateDisplay();
    }

    public void checkMinRoll(object sender, eventArgs e)
    {
        BattleManager bm = sender as BattleManager;
        if (e.roll >= this.minRoll)
            bm.ModifyEnemyHP(-bm.player.damage);
        else bm.ModifyPlayerHP(-this.damage);
    }
}