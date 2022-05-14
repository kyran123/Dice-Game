using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DiceContainer : MonoBehaviour
{
    public TMP_Text text;
    public Die currentDie;
    public List<int> displaySides;
    public TMP_Text price;
    public int diePrice;

    public int calculatePrice(List<int> list)
    {
        int price = 0;
        foreach(int i in list)
        {
            price+=i;
        }
        return (int)Mathf.Floor(price * 1.2f);
    }

    public void addDie(Die die)
    {
        this.currentDie = die;
        string arr = "";
        foreach (Side side in die.sides)
        {
            arr += side.getValue();
            arr += " | ";
        }
        text.text = arr.Remove(arr.Length - 3);
    }

    public void addDie(List<int> list)
    {
        string arr = "";
        foreach (int side in list)
        {
            switch((SideValue)side) {
                case SideValue.one:
                case SideValue.two:
                case SideValue.three:
                case SideValue.four:
                case SideValue.five: 
                case SideValue.seven: 
                case SideValue.eight:
                    arr += side.ToString();
                    break; 
                case SideValue.six:                     
                case SideValue.nine:
                    arr += $"{side.ToString()}.";
                    break; 
                case SideValue.doOneDamage:
                    arr += "⚔";
                    break;
                case SideValue.doTwoDamage:
                    arr += "⚔⚔";
                    break;
                case SideValue.healOne:
                    arr += "❤";
                    break;
                case SideValue.healTwo:
                    arr += "❤❤";
                    break;
                case SideValue.takeOneDamage:
                    arr += "⚔";
                    break;
                case SideValue.takeTwoDamage:
                    arr += "⚔⚔";
                    break;
                case SideValue.getOneCoin:
                    arr += "₵";
                    break;
                case SideValue.getTwoCoins:
                    arr += "₵₵";
                    break;
                case SideValue.loseOneCoin:
                    arr += "₵";
                    break;
                case SideValue.loseTwoCoins:
                    arr += "₵₵";
                    break;
                default:
                    arr += "0";
                    break;
            }
            arr += " | ";
        }
        text.text = arr.Remove(arr.Length - 3);
        displaySides = list;
        diePrice = calculatePrice(this.displaySides);
        price.text = $"{diePrice}$";
        BattleManager._instance.updateShop();
    }

    public void remove()
    {
        BattleManager._instance.removeDie(this.currentDie);
        text.text = "";
        BattleManager._instance.updateShop();
    }
}
