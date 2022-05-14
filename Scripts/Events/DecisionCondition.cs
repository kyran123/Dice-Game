using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DecisionCondition
{

    public conditionType type;

    public int coins;
    public int playerDamage;
    public int itemCount;
    public int dieCount;

    public skillOperator op;

    public bool validate()
    {
        switch(type)
        {
            case conditionType.Coin:
                return this.operatorCheck(this.coins, BattleManager._instance.player.coins);
            case conditionType.PlayerDamage:
                return this.operatorCheck(this.playerDamage, BattleManager._instance.player.getDamage());
            case conditionType.ItemCount:
                return this.operatorCheck(this.itemCount, BattleManager._instance.itemManager.itemCount());
            case conditionType.DieCount:
                return this.operatorCheck(this.dieCount, BattleManager._instance.GetComponent<DiceManager>().dieCount());
            default:
                return false;
        }
    }

    public bool operatorCheck(int condition, int value)
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
}

public enum conditionType
{
    Coin,
    PlayerDamage,
    ItemCount,
    DieCount
}
