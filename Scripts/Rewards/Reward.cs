using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Reward : MonoBehaviour
{
   void Start() {
        BattleManager._instance.OnEnemyDeath += this.addReward;
    }
   //coins, chance for items that goes up each time u dont get an item
   public int coins;
   public bool item;

   public void addReward(object sender, eventArgs e)
   {
      BattleManager._instance.giveReward(new eventArgs { coins = coins });
   }
}
