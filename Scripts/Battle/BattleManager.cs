using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class BattleManager : MonoBehaviour {
    
    public static BattleManager _instance { get; private set; }

    public GameObject cardContainer;
    public Player player;
    public int plusRolls = 0;

    //Event handlers
    public event EventHandler<eventArgs> OnBattle;          //When battle starts
    public event EventHandler<eventArgs> OnEnemyDamage;     //When enemy takes damage
    public event EventHandler<eventArgs> OnPlayerDamage;    //When player takes damage
    public event EventHandler<eventArgs> OnRoll;            //When player has rolled
    public event EventHandler<eventArgs> OnEnemyDeath;      //When enemy dies
    public event EventHandler<eventArgs> OnReward;          //When reward is given to player
    public event EventHandler<eventArgs> OnModifyCoins;     //Modify coins event
    public event EventHandler<eventArgs> OnAddItem;         //Add item event
    public event EventHandler<eventArgs> OnAddItemToHand;   //Add item to hand event
    public event EventHandler<eventArgs> OnRewardAdded;     //When item has been added

    void Awake() {
        if (_instance != null && _instance != this) { 
            Destroy(this);
        } else { 
            _instance = this;
        } 
    }

    public void enemyDeath(Enemy Enemy)
    {
        this.OnEnemyDeath?.Invoke(Enemy, new eventArgs {});
        Destroy(Enemy.gameObject);
    }

    public void showNewItem(GameObject item) {
        this.OnAddItem?.Invoke(this, new eventArgs { itemObject = item });
    }

    public void addItemToHand(Item item) {
        this.OnAddItemToHand?.Invoke(item, new eventArgs { itemObject = item.gameObject });
    }

    public void giveReward(eventArgs args)
    {
        this.OnReward?.Invoke(this, args);
    }

    public void rewardAdded() {
        this.OnRewardAdded?.Invoke(this, new eventArgs {});
    }

    public void OnDiceRoll(List<int> rolls)
    {
        if(plusRolls > 0)
        {
            rolls = rolls.Select(roll => {
                if(roll < 6) return roll + plusRolls;
                else return roll;
            }).ToList();
            plusRolls = 0;
        }
        rolls.ForEach(roll => Debug.Log(roll));
        int totalValue = rolls.Sum();
        this.OnRoll?.Invoke(this, new eventArgs { roll = totalValue, individualRolls = rolls });
    }

    public void attackEnemy(int value) 
    {
        this.OnEnemyDamage?.Invoke(this, new eventArgs { damage = value } );
    }

    public void ModifyPlayerHP(int value)
    {
        this.OnPlayerDamage?.Invoke(this, new eventArgs { damage = value } );
    }

    public void modifyCoins(int value) 
    {
        this.OnModifyCoins?.Invoke(this, new eventArgs { coins = value });
    }
}

public class eventArgs : EventArgs {
    public int roll;
    public int damage;
    public int coins;
    public bool item;
    public GameObject itemObject;
    public List<int> individualRolls;
}