using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.SceneManagement;
using System.Text;

public class BattleManager : MonoBehaviour
{

    public static BattleManager _instance { get; private set; }

    public GameObject cardContainer;
    public Player player;
    public EnemyManager em;
    public DebugController dc;
    public ItemManager itemManager;
    public NewItemManager nItemManager;
    public int plusRolls = 0;
    public int level = 1;
    public int maxLevel = 20;
    public int easyRange, mediumRange, hardRange;

    public screen screenState = screen.Event;

    //Event handlers
    public event EventHandler<eventArgs> OnBattle;          //When battle starts
    public event EventHandler<eventArgs> OnEnemyDamage;     //When enemy takes damage
    public event EventHandler<eventArgs> OnPlayerDamage;    //When player takes damage
    public event EventHandler<eventArgs> OnRoll;            //When player has rolled
    public event EventHandler<eventArgs> OnRollDamage;      //When player has rolled and enemy needs to figure out damage
    public event EventHandler<eventArgs> OnEnemyDeath;      //When enemy dies
    public event EventHandler<eventArgs> OnPlayerDeath;     //When player dies
    public event EventHandler<eventArgs> OnReward;          //When reward is given to player
    public event EventHandler<eventArgs> OnModifyCoins;     //Modify coins event
    public event EventHandler<eventArgs> OnAddItem;         //Add item event
    public event EventHandler<eventArgs> OnAddItemToHand;   //Add item to hand event
    public event EventHandler<eventArgs> OnRewardAdded;     //When item has been added
    public event EventHandler<eventArgs> OnRemoveRandomItem;//When removing random item
    public event EventHandler<eventArgs> OnModifyEnemyDamage;//When modifying enemy damage
    public event EventHandler<eventArgs> OnModifyPlayerDamage;//When modifying player damage
    public event EventHandler<eventArgs> OnModifyMinRolls;  //When modifying minRolls
    public event EventHandler<eventArgs> OnChangeMinRoll;   //When setting minRolls
    public event EventHandler<eventArgs> OnTurn;            //When turn ends
    public event EventHandler<eventArgs> OnAddcurseItem;    //When curse is added to hand
    public event EventHandler<eventArgs> OnDeleteCyclingHelmet; //when cycling helmet is used

    public event EventHandler<eventArgs> OnGenerateEnemy;   //When a new enemy is generated
    public event EventHandler<eventArgs> OnAddEventItem;    //When an event adds an item to hand
    public event EventHandler<eventArgs> OnRedrawHand;      //When an event redraws the hand
    public event EventHandler<eventArgs> OnHandIsFull;      //When the hand is full... or not?
    public event EventHandler<eventArgs> OnAddDie;          //When adding a die
    public event EventHandler<eventArgs> OnRemoveDie;       //When removing a die
    public event EventHandler<eventArgs> OnToggleCurse;     //When toggling item removability
    public event EventHandler<eventArgs> OnRemoveItem;      //When removing an item
    public event EventHandler<eventArgs> OnUpdateShop;      //When the shop gets updated
    public event EventHandler<eventArgs> OnClearPaths;

    public event EventHandler<eventArgs> OnToggleScreen;    //Toggling screen

    //Debug Event handlers
    public event EventHandler<eventArgs> debugSetPlayerHP;
    public event EventHandler<eventArgs> debugSetPlayerDamage;
    public event EventHandler<eventArgs> debugSetEnemyHP;
    public event EventHandler<eventArgs> debugSetEnemyDamage;
    public event EventHandler<eventArgs> debugSetEnemyMinRoll;
    public event EventHandler<eventArgs> debugKillEnemy;
    public event EventHandler<eventArgs> debugAddItem;
    public event EventHandler<eventArgs> debugSetCoins;
    public event EventHandler<eventArgs> debugDestroyEnemy;

    public event EventHandler<eventArgs> debugMessage;
    public event EventHandler<eventArgs> debugInspect;


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

    public void nextTurn()
    {
        this.level++;
        if (this.level == this.maxLevel)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void addDie(List<int> sides)
    {
        this.OnAddDie?.Invoke(this, new eventArgs { individualRolls = sides });
    }

    public void playerDeath()
    {
        this.toggleScreen(screen.GameOver);
    }

    public void removeDie(Die d)
    {
        this.OnRemoveDie?.Invoke(this, new eventArgs { die = d });
    }

    public void updateShop()
    {
        this.OnUpdateShop?.Invoke(this, new eventArgs { });
    }

    public void generateEnemy(bool elite = false)
    {
        this.OnGenerateEnemy?.Invoke(this, new eventArgs { isElite = elite });
        this.inspect(this.dc.inspectValue);
    }

    public void generateEnemy(string name)
    {
        this.OnGenerateEnemy?.Invoke(this, new eventArgs { debug_string = name });
    }

    public void enemyDeath(Enemy enemy)
    {
        this.OnEnemyDeath?.Invoke(enemy, new eventArgs { });
        bool hasItemReward = enemy.itemReward;
        if (!hasItemReward) this.toggleScreen(screen.Path);
        else this.itemManager.getRandomItem();
        this.destroyEnemy(enemy);
    }

    public void destroyEnemy(Enemy enemy)
    {
        Destroy(enemy.gameObject);
        this.inspect("");
    }

    public void showNewItem(GameObject item)
    {
        this.OnAddItem?.Invoke(this, new eventArgs { itemObject = item });
    }

    public void addCurseItem()
    {
        this.OnAddcurseItem?.Invoke(this, new eventArgs { });
    }

    public void addItemToHand(Item item)
    {
        this.OnAddItemToHand?.Invoke(item, new eventArgs { itemObject = item.gameObject });
        this.nItemManager.ItemContainer.item = null;
    }

    public void removeRandomItem()
    {
        this.OnRemoveRandomItem?.Invoke(this, new eventArgs { });
        this.inspect(this.dc.inspectValue);
    }

    public void removeRandomNonCurseItem() 
    {
        this.OnRemoveRandomItem?.Invoke(this, new eventArgs { item = true });
    }

    public void removeCyclingHelmet()
    {
        this.OnDeleteCyclingHelmet?.Invoke(this, new eventArgs { });
    }

    public void updateShopItems()
    {
        this.OnRemoveItem?.Invoke(this, new eventArgs { });
    }

    public void giveReward(eventArgs args)
    {
        this.OnReward?.Invoke(this, args);
        this.inspect(this.dc.inspectValue);
    }

    public void rewardAdded()
    {
        this.OnRewardAdded?.Invoke(this, new eventArgs { });
    }

    public void addEventItem(GameObject item)
    {
        this.OnAddEventItem?.Invoke(this, new eventArgs { itemObject = item });
    }

    public void addNonCurseEventItem()
    {
        this.OnAddEventItem?.Invoke(this, new eventArgs { startEvent = true });
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
                if (roll < 9) return roll + plusRolls;
                else return roll;
            }).ToList();
            plusRolls = 0;
        }
        //rolls.ForEach(roll => Debug.Log(roll));
        int totalValue = rolls.Sum();
        this.OnRoll?.Invoke(this, new eventArgs { roll = totalValue, individualRolls = rolls });
        this.OnRollDamage?.Invoke(this, new eventArgs { roll = totalValue, individualRolls = rolls });
        this.OnTurn?.Invoke(this, new eventArgs { });
    }

    public void ModifyEnemyHP(int value)
    {
        this.OnEnemyDamage?.Invoke(this, new eventArgs { damage = value });
        this.inspect(this.dc.inspectValue);
    }

    public void modifyEnemyDamage(int value)
    {
        this.OnModifyEnemyDamage?.Invoke(this, new eventArgs { damage = value });
        this.inspect(this.dc.inspectValue);
    }

    public void modifyPlayerDamage(int value)
    {
        this.OnModifyPlayerDamage?.Invoke(this, new eventArgs { damage = value });
        this.inspect(this.dc.inspectValue);
    }

    public void modifyMinRolls(int value)
    {
        this.OnModifyMinRolls?.Invoke(this, new eventArgs { roll = value });
        this.inspect(this.dc.inspectValue);
    }

    public void changeMinRoll(int value)
    {
        this.OnChangeMinRoll?.Invoke(this, new eventArgs { roll = value });
        this.inspect(this.dc.inspectValue);
    }

    public void ModifyPlayerHP(int value)
    {
        this.OnPlayerDamage?.Invoke(this, new eventArgs { damage = value });
        this.inspect(this.dc.inspectValue);
    }

    public void modifyCoins(int value)
    {
        this.OnModifyCoins?.Invoke(this, new eventArgs { coins = value });
        this.inspect(this.dc.inspectValue);
    }

    public void toggleScreen(screen screen, string name = null)
    {
        this.screenState = screen;
        if (screen == screen.Battle)
        {
            this.OnBattle?.Invoke(this, new eventArgs { });
        }
        else
        {
            this.debugDestroyEnemy?.Invoke(this, new eventArgs { });
        }
        if(screen != screen.Path) this.OnClearPaths?.Invoke(this, new eventArgs { });
        this.OnToggleScreen?.Invoke(this, new eventArgs { screenValue = screen, debug_string = name });
    }

    public void toggleCurse()
    {
        this.OnToggleCurse?.Invoke(this, new eventArgs { });
    }

    //
    // DEBUG FUNCTIONS
    //
    public void setPlayerHP(int value) { this.debugSetPlayerHP?.Invoke(this, new eventArgs { debug_int = value }); this.inspect(this.dc.inspectValue); }
    public void setEnemyHP(int value) { this.debugSetEnemyHP?.Invoke(this, new eventArgs { debug_int = value }); this.inspect(this.dc.inspectValue); }
    public void setEnemyDamage(int value) { this.debugSetEnemyDamage?.Invoke(this, new eventArgs { debug_int = value }); this.inspect(this.dc.inspectValue); }
    public void setEnemyRoll(int value) { this.debugSetEnemyMinRoll?.Invoke(this, new eventArgs { debug_int = value }); this.inspect(this.dc.inspectValue); }
    public void KillEnemy() { this.debugKillEnemy?.Invoke(this, new eventArgs { }); this.inspect("close"); }
    public void addItem(string value) { this.debugAddItem?.Invoke(this, new eventArgs { debug_string = value }); this.inspect(this.dc.inspectValue); }
    public void setCoins(int value) { this.debugSetCoins?.Invoke(this, new eventArgs { debug_int = value }); this.inspect(this.dc.inspectValue); }
    public void setDamage(int value) { this.debugSetPlayerDamage?.Invoke(this, new eventArgs { debug_int = value }); this.inspect(this.dc.inspectValue); }
    public void spawnEnemy(string name) 
    {
        BattleManager._instance.generateEnemy(name);
    }
    public void message(string msg) { this.debugMessage?.Invoke(this, new eventArgs { debug_string = msg }); }
    public void inspect(string msg)
    {
        if(this.compareStrings(msg, "player"))
        {
            this.debugInspect?.Invoke(this, new eventArgs { debug_player = this.player, debug_items = this.itemManager.getAllItems() });
        }
        if(this.compareStrings(msg, "enemy"))
        {
            if(em.currentEnemy != null) this.debugInspect?.Invoke(this, new eventArgs { debug_enemy = em.currentEnemy.GetComponent<Enemy>() });
            else this.message("No enemy on board");
        }
        if(this.compareStrings(msg, "close"))
        {
            this.debugInspect?.Invoke(this, new eventArgs { debug_string = "close" });
        }
    }

    public bool compareStrings(string a, string b)
    {
        string x = new StringBuilder(a.ToLower()).ToString();
        string y = new StringBuilder(b.ToLower()).ToString();
        if(string.Equals(x, y)) return true;
        return false; 
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
    public bool isElite;
    //Debug values
    public int debug_int;
    public string debug_string;
    public Enemy debug_enemy;
    public Player debug_player;
    public List<Item> debug_items;
}

public enum screen
{
    Battle,
    Path,
    Event,
    Shop,
    GameOver
}