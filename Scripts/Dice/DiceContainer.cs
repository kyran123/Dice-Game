using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DiceContainer : MonoBehaviour
{
    public TMP_Text text;
    public Die currentDie;

    public void addDie(Die die)
    {
        this.currentDie = die;
        string arr = "Dice: ";
        foreach (Side side in die.sides)
        {
            arr += (int)side.value;
            arr += " | ";
        }
        text.text = arr.Remove(arr.Length - 3);
    }

    public void remove()
    {
        BattleManager._instance.removeDie(this.currentDie);
        text.text = "";
    }
}
