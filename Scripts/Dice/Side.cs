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
        sideText.text = ((int)newValue).ToString();
    }

    public int getValue() {
        switch(value) {
            case SideValue.one:
            case SideValue.two:
            case SideValue.three:
            case SideValue.four:
            case SideValue.five: 
            case SideValue.six: 
            case SideValue.seven: 
            case SideValue.eight: 
            case SideValue.nine: 
                return (int)value;
            default:
                return 0;
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