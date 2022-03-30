using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class Die : MonoBehaviour
{
    public List<Side> sides = new List<Side>();
    public bool logged = true;

    public int getSide()
    {
        float highestSide = 0;
        int value = 0;
        foreach (Side side in this.sides)
        {
            if (side.transform.position.y > highestSide)
            {
                highestSide = side.transform.position.y;
                BattleManager bm = BattleManager._instance;
                switch(side.value)
                {
                    case SideValue.doOneDamage:
                        bm.ModifyEnemyHP(-1);
                        break;
                    case SideValue.doTwoDamage:
                        bm.ModifyEnemyHP(-2);
                        break;
                    case SideValue.getOneCoin:
                        bm.modifyCoins(1);
                        break;
                    case SideValue.getTwoCoins:
                        bm.modifyCoins(2);
                        break;
                    case SideValue.loseOneCoin:
                        bm.modifyCoins(-1);
                        break;
                    case SideValue.loseTwoCoins:
                        bm.modifyCoins(-2);
                        break;
                    case SideValue.healOne:
                        bm.ModifyPlayerHP(1);
                        break;
                    case SideValue.healTwo:
                        bm.ModifyPlayerHP(2);
                        break;
                    case SideValue.takeOneDamage:
                        bm.ModifyPlayerHP(-1);
                        break;
                    case SideValue.takeTwoDamage:
                        bm.ModifyPlayerHP(-2);
                        break;
                }
                value = side.getValue();
            }
        }
        return value;
    }

    public void updateSides(List<SideValue> newSides)
    {
        for (int i = 0; i < newSides.Count; i++)
        {
            this.sides[i].updateValue(newSides[i]);
        }
    }
}