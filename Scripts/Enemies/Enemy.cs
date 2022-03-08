using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour{

    private int HP;
    int damage { get;}
    private int MinRoll;

    // positive -> healing , negative -> damage
    public void ModifyHP(int value)
    {
        HP += value;
        if(HP<=0)
        {
            //die
            //check for player death
        }
    }

    public bool checkMinRoll(int value)
    {
        if(value >= MinRoll)
        return true;
        else return false;
    }

}