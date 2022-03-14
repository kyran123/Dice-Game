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
    public event EventHandler<eventArgs> checkMinRoll;

    void Awake() {
        if (_instance != null && _instance != this) { 
            Destroy(this);
        } else { 
            _instance = this;
        } 
    }

    public void attack(List<int> rolls) {
        int totalValue = rolls.Sum();
        this.checkMinRoll?.Invoke(this, new eventArgs { roll = totalValue });
    }

    public void attackEnemy(int value) { 
        this.OnEnemyDamage?.Invoke(this, new eventArgs { damage = -value } );
    }

    public void attackPlayer() {
        this.OnPlayerDamage?.Invoke(this, new eventArgs { damage = -this.player.damage } );
    }
}

public class eventArgs : EventArgs {
    public int roll;
    public int damage;
}