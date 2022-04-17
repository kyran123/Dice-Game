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
        string arr = "Dice: ";
        foreach (Side side in die.sides)
        {
            arr += (int)side.value;
            arr += " | ";
        }
        text.text = arr.Remove(arr.Length - 3);
    }

    public void addDie(List<int> list)
    {
        string arr = "Dice: ";
        foreach (int side in list)
        {
            arr += side;
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
