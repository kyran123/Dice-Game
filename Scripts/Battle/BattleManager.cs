using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class BattleManager : MonoBehaviour {
    
    public static BattleManager _instance { get; private set; }

    public GameObject cardContainer;
    public Player player;

    //Event handlers
    public event EventHandler<eventArgs> OnEnemyDamage;
    public event EventHandler<eventArgs> OnPlayerDamage;
    public event EventHandler<eventArgs> OnRoll;
    public event EventHandler<eventArgs> OnEnemyDeath;
    public event EventHandler<eventArgs> OnReward;

    void Awake() {
        if (_instance != null && _instance != this) { 
            Destroy(this);
        } else { 
            _instance = this;
        } 
    }

    public void enemyDeath()
    {
        this.OnEnemyDeath?.Invoke(this, new eventArgs {});
    }

    public void giveReward(eventArgs args)
    {
        this.OnReward?.Invoke(this, args);
    }

    public void attack(List<int> rolls) 
    {
        int totalValue = rolls.Sum();
        this.OnRoll?.Invoke(this, new eventArgs { roll = totalValue });
    }

    public void attackEnemy(int value) 
    {
        this.OnEnemyDamage?.Invoke(this, new eventArgs { damage = value } );
    }

    public void ModifyPlayerHP(int value) 
    {
        this.OnPlayerDamage?.Invoke(this, new eventArgs { damage = value } );
    }
}

public class eventArgs : EventArgs {
    public int roll;
    public int damage;
    public int coins;
    public bool item;
}