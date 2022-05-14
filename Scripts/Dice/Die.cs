using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class Die : MonoBehaviour
{
    public List<Side> sides = new List<Side>();
    public bool logged = true;

    public void convertIntToSideValues(List<int> list)
    {
        foreach (int i in list)
        {
            Side side = new Side();
            side.value = (SideValue)i;
            sides.Add(side);
        }
    }

    public List<int> convertSidesToIntList() 
    {
        List<int> intList = new List<int>();
        foreach(Side side in this.sides)
        {
            intList.Add((int)side.value);
        }
        return intList;
    }

    public int getSide()
    {
        BattleManager bm = BattleManager._instance;
        Side rolled = this.sides[0];
        float highestSide = 0;
        foreach(Side side in this.sides)
        {
            if (side.transform.position.y > highestSide)
            {
                highestSide = side.transform.position.y;
                rolled = side;
            }
        }
        switch (rolled.value)
        {
            case SideValue.one:
            case SideValue.two:
            case SideValue.three:
            case SideValue.four:
            case SideValue.five: 
            case SideValue.six: 
            case SideValue.seven: 
            case SideValue.eight: 
            case SideValue.nine: 
                return (int)rolled.value;
            case SideValue.doOneDamage:
                bm.ModifyEnemyHP(-1);
                return 0;
            case SideValue.doTwoDamage:
                bm.ModifyEnemyHP(-2);
                return 0;
            case SideValue.getOneCoin:
                bm.modifyCoins(1);
                return 0;
            case SideValue.getTwoCoins:
                bm.modifyCoins(2);
                return 0;
            case SideValue.loseOneCoin:
                bm.modifyCoins(-1);
                return 0;
            case SideValue.loseTwoCoins:
                bm.modifyCoins(-2);
                return 0;
            case SideValue.healOne:
                bm.ModifyPlayerHP(1);
                return 0;
            case SideValue.healTwo:
                bm.ModifyPlayerHP(2);
                return 0;
            case SideValue.takeOneDamage:
                bm.ModifyPlayerHP(-1);
                return 0;
            case SideValue.takeTwoDamage:
                bm.ModifyPlayerHP(-2);
                return 0;
            default:
                return 0;
        }
    }

    public void updateSides(List<SideValue> newSides)
    {
        for (int i = 0; i < newSides.Count; i++)
        {
            this.sides[i].updateValue(newSides[i]);
        }
    }
}