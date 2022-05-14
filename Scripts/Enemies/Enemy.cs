using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Enemy : MonoBehaviour
{

    public EnemyDisplay display;

    [SerializeField]
    public string enemyName;
    public int HP;
    public int damage;
    public int minRoll;
    [Range(1, 4), Tooltip("1 -> Easy, 2 -> Medium, 3 -> Hard, 4 -> Elite")]
    public int difficultyValue;

    public int coinReward;
    public bool itemReward;

    [HideInInspector]
    public List<GameObject> allEnemies = new List<GameObject>();

    public void subscribe()
    {
        BattleManager._instance.OnEnemyDamage += this.ModifyHP;
        BattleManager._instance.OnRollDamage += this.checkMinRoll;
        BattleManager._instance.OnModifyEnemyDamage += this.ModifyDamage;
        BattleManager._instance.OnModifyMinRolls += this.ModifyMinRoll;
        BattleManager._instance.OnChangeMinRoll += this.ChangeMinRoll;
        //Debug events
        BattleManager._instance.debugKillEnemy += this.death;
        BattleManager._instance.debugSetEnemyHP += this.setHP;
        BattleManager._instance.debugDestroyEnemy += this.destroy;
        BattleManager._instance.debugSetEnemyDamage += this.setDamage;
        BattleManager._instance.debugSetEnemyMinRoll += this.setMinRoll;
        EnemySkills[] skills = this.GetComponents<EnemySkills>();
        foreach (EnemySkills skill in skills)
        {
            skill.subscribe();
        }
    }

    public int getDamage()
    {
        return this.damage + BattleManager._instance.itemManager.getItemsValue(Items.ScratchedKnee);
    }

    public void setDamage(object sender, eventArgs e)
    {
        this.damage = e.debug_int;
        this.display.updateDisplay();
    }

    public int getMinRoll()
    {
        return this.minRoll + BattleManager._instance.itemManager.getItemsValue(Items.RustySword);
    }

    public void setMinRoll(object sender, eventArgs e)
    {
        this.minRoll = e.debug_int;
        this.display.updateDisplay();
    }

    public void setHP(object sender, eventArgs e)
    {
        this.HP = e.debug_int;
        this.display.updateDisplay();
        if (HP <= 0)
        {
            this.death(this, new eventArgs { });
        }
    }

    // positive -> healing , negative -> damage
    public void ModifyHP(object sender, eventArgs e)
    {
        BattleManager bm = sender as BattleManager;
        HP += e.damage;
        this.display.updateDisplay();
        if (HP <= 0)
        {
            this.death(this, new eventArgs { });
        }
    }

    public void death(object sender, eventArgs e)
    {
        BattleManager bm = BattleManager._instance;
        if (coinReward != 0) bm.modifyCoins(coinReward);
        this.unSubscribe();
        bm.nextTurn();
        bm.enemyDeath(this);
    }

    public void destroy(object sender, eventArgs e)
    {
        this.unSubscribe();
        BattleManager._instance.destroyEnemy(this);
    }

    public void unSubscribe()
    {
        BattleManager._instance.OnEnemyDamage -= this.ModifyHP;
        BattleManager._instance.OnRollDamage -= this.checkMinRoll;
        BattleManager._instance.OnModifyEnemyDamage -= this.ModifyDamage;
        BattleManager._instance.OnModifyMinRolls -= this.ModifyMinRoll;
        BattleManager._instance.OnChangeMinRoll -= this.ChangeMinRoll;

        BattleManager._instance.debugKillEnemy -= this.death;
        BattleManager._instance.debugSetEnemyHP -= this.setHP;
        BattleManager._instance.debugDestroyEnemy -= this.destroy;
        BattleManager._instance.debugSetEnemyDamage -= this.setDamage;
        BattleManager._instance.debugSetEnemyMinRoll -= this.setMinRoll;
        List<EnemySkills> skills = this.GetComponents<EnemySkills>().ToList();
        if(skills.Count() > 0)
        {
            foreach (EnemySkills skill in skills)
            {
                skill.unSubscribe();
            }
        }
    }

    public List<EnemySkills> getEnemySkills()
    {
        return this.GetComponents<EnemySkills>().ToList();
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

    public void ChangeMinRoll(object sender, eventArgs e)
    {
        this.minRoll = e.roll;
        this.display.updateDisplay();
    }

    public void checkMinRoll(object sender, eventArgs e)
    {
        BattleManager bm = sender as BattleManager;
        if (e.roll >= this.getMinRoll())
            bm.ModifyEnemyHP(-bm.player.getDamage());
        else bm.ModifyPlayerHP(-this.getDamage());
    }
}