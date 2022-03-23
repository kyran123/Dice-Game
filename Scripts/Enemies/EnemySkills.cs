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
        BattleManager._instance.OnRoll += this.diceRollsCondition;
        BattleManager._instance.OnRoll += diceRollSumCondition;
        BattleManager._instance.OnRoll += this.diceNumCondition;
        this.playerHPCondition(this, new eventArgs { }); //Replace this with "start of battle" event
        this.enemyHPCondition(this, new eventArgs { }); //Replace this with "start of battle" event
        BattleManager._instance.OnEnemyDamage += this.enemyHPCondition;
        BattleManager._instance.OnPlayerDamage += this.playerHPCondition;
        BattleManager._instance.OnPlayerDamage += this.playerDamageCondition;
        BattleManager._instance.OnEnemyDamage += this.enemyDamageCondition;
    }

    [Header("Conditions")]
    public skillConditions conditions;
    public List<int> diceRolls;
    public int value;
    public skillOperator op;

    public bool operatorCheck(int condition, int value)
    {
        switch (op)
        {
            case skillOperator.equal:
                return condition == value;
            case skillOperator.lessThan:
                return condition < value;
            case skillOperator.moreThan:
                return condition > value;
            case skillOperator.lessOrEqual:
                return condition <= value;
            case skillOperator.moreOrequal:
                return condition >= value;
            default: return false;
        }
    }

    public void diceRollsCondition(object sender, eventArgs e)
    {
        e.individualRolls.Sort();
        diceRolls.Sort();
        if (e.individualRolls.SequenceEqual(diceRolls))
        {
            skillEffect();
        }
    }

    public void diceRollSumCondition(object sender, eventArgs e)
    {
        if (operatorCheck(e.roll, value))
        {
            skillEffect();
        }
    }

    public void diceNumCondition(object sender, eventArgs e)
    {
        if (operatorCheck(e.individualRolls.Count(), value))
        {
            skillEffect();
        }
    }

    public void playerHPCondition(object sender, eventArgs e)
    {
        if (operatorCheck(BattleManager._instance.player.HP, value))
        {
            skillEffect();
        }
    }

    public void enemyHPCondition(object sender, eventArgs e)
    {
        if (operatorCheck(this.GetComponent<Enemy>().HP, value))
        {
            skillEffect();
        }
    }

    public void playerDamageCondition(object sender, eventArgs e)
    {
        if (operatorCheck(e.damage, value))
        {
            skillEffect();
        }
    }

    public void enemyDamageCondition(object sender, eventArgs e)
    {
        if (operatorCheck(e.damage, value))
        {
            skillEffect();
        }
    }

    public void onDeathCondition(object sender, eventArgs e)
    {
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
    [Tooltip("Positive -> healing / adding, Negative -> damage / removing")]
    public int effectValue;

    public void skillEffect()
    {
        switch (effect)
        {
            case effect.playerHP:
                BattleManager._instance.ModifyPlayerHP(effectValue);
                break;
            case effect.enemyHP:
                BattleManager._instance.ModifyEnemyHP(effectValue);
                break;
            case effect.coins:
                BattleManager._instance.modifyCoins(effectValue);
                break;
            case effect.item:
                BattleManager._instance.removeRandomItem();
                break;
            case effect.enemyDamage:
                BattleManager._instance.modifyEnemyDamage(effectValue);
                break;
            case effect.minRoll:
                BattleManager._instance.modifyMinRolls(effectValue);
                break;
        }
    }
}

public enum skillConditions
{
    diceRolls,
    diceRollSum,
    diceNum,
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
    moreOrequal,
}

public enum effect
{
    playerHP,
    enemyHP,
    item,
    coins,
    enemyDamage,
    minRoll,
}