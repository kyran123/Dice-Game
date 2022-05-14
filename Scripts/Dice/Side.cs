using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class Side : MonoBehaviour
{
    public TMP_Text sideText;
    public SideValue value;

    public void updateValue(SideValue newValue)
    {
        value = newValue;
        switch(value)
        {
            case SideValue.takeOneDamage:
            case SideValue.takeTwoDamage:
            case SideValue.loseOneCoin:
            case SideValue.loseTwoCoins:
                sideText.color = Color.red;
                break;
            default:
                sideText.color = Color.black;
                break;

        }
        sideText.text = this.getValue();
    }

    public string getValue() {
        switch(value) {
            case SideValue.one:
            case SideValue.two:
            case SideValue.three:
            case SideValue.four:
            case SideValue.five: 
            case SideValue.seven: 
            case SideValue.eight: 
                return ((int)value).ToString();
            case SideValue.six: 
            case SideValue.nine: 
                return $"{((int)value).ToString()}.";
            case SideValue.doOneDamage:
                return "⚔";
            case SideValue.doTwoDamage:
                return "⚔⚔";
            case SideValue.healOne:
                return "❤";
            case SideValue.healTwo:
                return "❤❤";
            case SideValue.takeOneDamage:
                return "⚔";
            case SideValue.takeTwoDamage:
                return "⚔⚔";
            case SideValue.getOneCoin:
                return "₵";
            case SideValue.getTwoCoins:
                return "₵₵";
            case SideValue.loseOneCoin:
                return "₵";
            case SideValue.loseTwoCoins:
                return "₵₵";
            default:
                return "0";
        }
    }
}

public enum SideValue
{
    zero,
    one,
    two,
    three,
    four,
    five,
    six,
    seven,
    eight,
    nine,
    doOneDamage,
    doTwoDamage,
    healOne,
    healTwo,
    takeOneDamage,
    takeTwoDamage,
    getOneCoin,
    getTwoCoins,
    loseOneCoin,
    loseTwoCoins
}