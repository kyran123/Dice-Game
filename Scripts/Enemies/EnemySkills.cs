using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemySkills : MonoBehaviour
{
    /* What are skills based on
        -dice rolls
        -total dice rolls
        -player hp
        -enemy hp
        -on enemy death
        -number of dice
        -on player damage (when enemy attacks player)
    */

    void Start()
    {
        
    }

    public void subscribe()
    {
        BattleManager._instance.OnTurn += this.turnCondition;
        BattleManager._instance.OnRoll += this.diceRollsCondition;
        BattleManager._instance.OnRoll += this.diceRollCondition;
        BattleManager._instance.OnRoll += this.diceRollSumCondition;
        BattleManager._instance.OnBattle += this.diceNumCondition;
        BattleManager._instance.OnBattle += this.OnStartBattle;
        BattleManager._instance.OnEnemyDamage += this.enemyHPCondition;
        BattleManager._instance.OnPlayerDamage += this.playerHPCondition;
        BattleManager._instance.OnPlayerDamage += this.playerDamageCondition;
        BattleManager._instance.OnEnemyDamage += this.enemyDamageCondition;
        if (this.conditions == skillConditions.diceNum) this.oncePerBattle = true;
    }

    public void unSubscribe()
    {
        BattleManager._instance.OnTurn -= this.turnCondition;
        BattleManager._instance.OnRoll -= this.diceRollsCondition;
        BattleManager._instance.OnRoll -= this.diceRollCondition;
        BattleManager._instance.OnRoll -= this.diceRollSumCondition;
        BattleManager._instance.OnBattle -= this.diceNumCondition;
        BattleManager._instance.OnBattle -= this.OnStartBattle;
        BattleManager._instance.OnEnemyDamage -= this.enemyHPCondition;
        BattleManager._instance.OnPlayerDamage -= this.playerHPCondition;
        BattleManager._instance.OnPlayerDamage -= this.playerDamageCondition;
        BattleManager._instance.OnEnemyDamage -= this.enemyDamageCondition;
    }

    [Header("Conditions")]
    public skillConditions conditions;
    public List<int> diceRolls;
    public int value;
    public skillOperator op;

    [Space(10)]
    private bool used;
    public bool oncePerBattle;

    private int lastValue;

    public void OnStartBattle(object sender, eventArgs e)
    {
        this.used = false;
        this.playerHPCondition(this, new eventArgs { });
        this.enemyHPCondition(this, new eventArgs { });
    }

    public bool operatorCheck(int value, int condition)
    {
        switch (op)
        {
            case skillOperator.equal:
                return value == condition;
            case skillOperator.lessThan:
                return value < condition;
            case skillOperator.moreThan:
                return value > condition;
            case skillOperator.lessOrEqual:
                return value <= condition;
            case skillOperator.moreOrEqual:
                return value >= condition;
            default: return false;
        }
    }

    public bool isCondition(skillConditions type)
    {
        bool con = true;
        if (this.oncePerBattle && this.used) con = false;
        return type == this.conditions && con;
    }

    public void turnCondition(object sender, eventArgs e)
    {
        if (!this.isCondition(skillConditions.turn)) return;
        skillEffect();
    }

    public void diceRollsCondition(object sender, eventArgs e)
    {
        if (!this.isCondition(skillConditions.diceRolls)) return;
        //List<int> rolls = diceRolls.ToList();
        foreach (int roll in e.individualRolls)
        {
            if (diceRolls.Contains(roll))
            {
                skillEffect();
            }
        }
    }

    public void diceRollCondition(object sender, eventArgs e)
    {
        if (!this.isCondition(skillConditions.diceRoll)) return;
        if (e.individualRolls.Contains(value))
        {
            skillEffect();
        }
    }

    public void diceRollSumCondition(object sender, eventArgs e)
    {
        if (!this.isCondition(skillConditions.diceRollSum)) return;
        if (operatorCheck(e.roll, value))
        {
            skillEffect();
        }
    }

    public void diceNumCondition(object sender, eventArgs e)
    {
        if (!this.isCondition(skillConditions.diceNum)) return;
        if (operatorCheck(BattleManager._instance.GetComponent<DiceManager>().dieCount(), value))
        {
            skillEffect();
        }
    }

    public void playerHPCondition(object sender, eventArgs e)
    {
        if (!this.isCondition(skillConditions.playerHP)) return;
        if (operatorCheck(BattleManager._instance.player.HP, value))
        {
            //Conditions met
            this.skillEffect();
            this.used = true;
        }
        else
        {
            //Conditions not met
            if(this.used == true) 
            {
                this.undoSkillEffect();
                this.used = false;
            }
        }
    }

    public void enemyHPCondition(object sender, eventArgs e)
    {
        if (!this.isCondition(skillConditions.enemyHP)) return;
        if (operatorCheck(this.GetComponent<Enemy>().HP, value))
        {
            //Conditions met
            this.skillEffect();
            this.used = true;
        }
        else
        {
            //Conditions not met
            if(this.used == true) 
            {
                this.undoSkillEffect();
                this.used = false;
            }
        }
    }

    public void playerDamageCondition(object sender, eventArgs e)
    {
        if (!this.isCondition(skillConditions.onPlayerDamage)) return;
        if (operatorCheck(e.damage, value))
        {
            skillEffect();
        }
    }

    public void enemyDamageCondition(object sender, eventArgs e)
    {
        if (!this.isCondition(skillConditions.onEnemyDamage)) return;
        if (operatorCheck(e.damage, value))
        {
            skillEffect();
        }
    }

    public void onDeathCondition(object sender, eventArgs e)
    {
        if (!this.isCondition(skillConditions.onDeath)) return;
        if (this.GetComponent<Enemy>() == null)
        {
            skillEffect();
        }
        else if (operatorCheck(this.GetComponent<Enemy>().HP, value))
        {
            skillEffect();
        }
    }

    /*
        -modify player HP
        -modify enemy HP
        -remove Item
        -modify coins
        -modify enemy damage
        -modify minRoll
    */

    [Header("Effects")]
    public effect effect;
    public itemEffect iEffect;
    [Tooltip("Positive -> healing / adding, Negative -> damage / removing")]
    public int effectValue;
    public int min, max;

    public void skillEffect()
    {
        switch (effect)
        {
            case effect.playerHP:
                BattleManager._instance.ModifyPlayerHP(effectValue);
                break;
            case effect.enemyHP:
                if (effectValue == 0) BattleManager._instance.ModifyEnemyHP(Random.Range(min, max));
                else BattleManager._instance.ModifyEnemyHP(effectValue);
                break;
            case effect.coins:
                BattleManager._instance.modifyCoins(effectValue);
                break;
            case effect.item:
                switch (iEffect)
                {
                    case itemEffect.RemoveRandom:
                        BattleManager._instance.removeRandomItem();
                        break;
                    case itemEffect.GiveCurse:
                        BattleManager._instance.addCurseItem();
                        break;
                }
                break;
            case effect.enemyDamage:
                BattleManager._instance.modifyEnemyDamage(effectValue);
                break;
            case effect.minRoll:
                if (effectValue == 0)
                {
                    this.lastValue = Random.Range(min, max);
                    BattleManager._instance.changeMinRoll(this.lastValue);
                }
                else
                {
                    this.lastValue = this.effectValue;
                    BattleManager._instance.modifyMinRolls(this.lastValue);
                }
                break;
        }
        if (this.oncePerBattle) this.used = true;
    }

    public void undoSkillEffect()
    {
        switch (effect)
        {
            case effect.coins:
                BattleManager._instance.modifyCoins(-this.effectValue);
                break;
            case effect.enemyDamage:
                BattleManager._instance.modifyEnemyDamage(-this.effectValue);
                break;
            case effect.minRoll:
                BattleManager._instance.modifyMinRolls(-this.lastValue);
                break;
        }
        if (this.oncePerBattle) this.used = false;
    }
}

public enum skillConditions
{
    turn,
    diceRolls, //If multiple / all dice meet the conditions
    diceRoll, //If singular dice meets this condition
    diceRollSum, //Total sum of the dice
    diceNum, //Total amount of dice
    playerHP,
    enemyHP,
    onPlayerDamage,
    onEnemyDamage,
    onDeath,
}

public enum skillOperator
{
    equal,
    lessThan,
    moreThan,
    lessOrEqual,
    moreOrEqual,
}

public enum effect
{
    playerHP,
    enemyHP,
    item,
    coins,
    enemyDamage,
    minRoll
}

public enum itemEffect
{
    RemoveRandom,
    GiveCurse
}