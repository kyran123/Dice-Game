using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class BattleManager : MonoBehaviour
{

    public static BattleManager _instance { get; private set; }

    public GameObject cardContainer;
    public Player player;
    public ItemManager itemManager;
    public int plusRolls = 0;
    public int level = 1;
    public int easyRange, mediumRange, hardRange;

    public screen screenState;

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
    public event EventHandler<eventArgs> OnRemoveRandomItem;//When removing random item
    public event EventHandler<eventArgs> OnModifyEnemyDamage;//When modifying enemy damage
    public event EventHandler<eventArgs> OnModifyPlayerDamage;//When modifying player damage
    public event EventHandler<eventArgs> OnModifyMinRolls;  //When modifying minRolls

    public event EventHandler<eventArgs> OnGenerateEnemy;   //When a new enemy is generated
    public event EventHandler<eventArgs> OnAddEventItem;    //When an event adds an item to hand
    public event EventHandler<eventArgs> OnRedrawHand;      //When an event redraws the hand
    public event EventHandler<eventArgs> OnHandIsFull;      //When the hand is full... or not?
    public event EventHandler<eventArgs> OnAddDie;          //When adding a die
    public event EventHandler<eventArgs> OnRemoveDie;       //When removing a die
    public event EventHandler<eventArgs> OnToggleCurse;     //When toggling item removability
    public event EventHandler<eventArgs> OnRemoveItem;      //When removing an item
    public event EventHandler<eventArgs> OnUpdateShop;      //When the shop gets updated

    public event EventHandler<eventArgs> OnToggleScreen;    //Toggling screen


    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
        }
        else
        {
            _instance = this;
        }
    }

    void Start()
    { //Temporary
        this.OnToggleScreen?.Invoke(this, new eventArgs { screenValue = screen.Event, startEvent = true });
    }

    public void addDie(List<int> sides)
    {
        this.OnAddDie?.Invoke(this, new eventArgs { individualRolls = sides });
    }

    public void removeDie(Die d)
    {
        this.OnRemoveDie?.Invoke(this, new eventArgs { die = d });
    }

    public void updateShop()
    {
        this.OnUpdateShop?.Invoke(this, new eventArgs { });
    }

    public void generateEnemy()
    {
        this.OnGenerateEnemy?.Invoke(this, new eventArgs { });
    }

    public void enemyDeath(Enemy enemy)
    {
        this.OnEnemyDeath?.Invoke(enemy, new eventArgs { });
        bool hasItemReward = enemy.itemReward;
        Destroy(enemy.gameObject);
        if (!hasItemReward) this.toggleScreen(screen.Path);
        else this.itemManager.getRandomItem(enemy);
    }

    public void showNewItem(GameObject item)
    {
        this.OnAddItem?.Invoke(this, new eventArgs { itemObject = item });
    }

    public void addItemToHand(Item item)
    {
        this.OnAddItemToHand?.Invoke(item, new eventArgs { itemObject = item.gameObject });
    }

    public void removeRandomItem()
    {
        this.OnRemoveRandomItem?.Invoke(this, new eventArgs { });
    }

    public void updateShopItems()
    {
        this.OnRemoveItem?.Invoke(this, new eventArgs { });
    }

    public void giveReward(eventArgs args)
    {
        this.OnReward?.Invoke(this, args);
    }

    public void rewardAdded()
    {
        this.OnRewardAdded?.Invoke(this, new eventArgs { });
    }

    public void addEventItem(GameObject item)
    {
        this.OnAddEventItem?.Invoke(this, new eventArgs { itemObject = item });
    }

    public void redrawHand()
    {
        this.OnRedrawHand?.Invoke(this, new eventArgs { });
    }

    public void handIsFull()
    {
        this.OnHandIsFull?.Invoke(this, new eventArgs { });
    }

    public void OnDiceRoll(List<int> rolls)
    {
        if (plusRolls > 0)
        {
            rolls = rolls.Select(roll =>
            {
                if (roll < 6) return roll + plusRolls;
                else return roll;
            }).ToList();
            plusRolls = 0;
        }
        rolls.ForEach(roll => Debug.Log(roll));
        int totalValue = rolls.Sum();
        this.OnRoll?.Invoke(this, new eventArgs { roll = totalValue, individualRolls = rolls });
    }

    public void ModifyEnemyHP(int value)
    {
        this.OnEnemyDamage?.Invoke(this, new eventArgs { damage = value });
    }

    public void modifyEnemyDamage(int value)
    {
        this.OnModifyEnemyDamage?.Invoke(this, new eventArgs { damage = value });
    }

    public void modifyPlayerDamage(int value)
    {
        this.OnModifyPlayerDamage?.Invoke(this, new eventArgs { damage = value });
    }

    public void modifyMinRolls(int value)
    {
        this.OnModifyMinRolls?.Invoke(this, new eventArgs { roll = value });
    }

    public void ModifyPlayerHP(int value)
    {
        this.OnPlayerDamage?.Invoke(this, new eventArgs { damage = value });
    }

    public void modifyCoins(int value)
    {
        this.OnModifyCoins?.Invoke(this, new eventArgs { coins = value });
    }

    public void toggleScreen(screen screen)
    {
        this.screenState = screen;
        this.OnToggleScreen?.Invoke(this, new eventArgs { screenValue = screen });
    }

    public void toggleCurse()
    {
        this.OnToggleCurse?.Invoke(this, new eventArgs { });
    }
}

public class eventArgs : EventArgs
{
    public int roll;
    public int damage;
    public int coins;
    public bool item;
    public GameObject itemObject;
    public List<int> individualRolls;
    public screen screenValue;
    public bool startEvent = false;
    public Die die;
}

public enum screen
{
    Battle,
    Path,
    Event,
    Shop,
    GameOver
}