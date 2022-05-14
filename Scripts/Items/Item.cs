using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public enum ItemType
{
    Passive,
    Active,
    Cursed
}

public class Item : MonoBehaviour, IPointerDownHandler
{
    void Start()
    {
        this.Description.text = this.generateDescription();
    }

    public void subscribe()
    {
        BattleManager._instance.OnRoll += this.bagOfChips;
        BattleManager._instance.OnRoll += this.looseChange;
        BattleManager._instance.OnRoll += this.butterFingers;
        BattleManager._instance.OnPlayerDamage += this.brokenPiggybank;
        BattleManager._instance.OnPlayerDamage += this.tornPocket;
        BattleManager._instance.OnPlayerDamage += this.scratchedKnee;
        BattleManager._instance.OnPlayerDamage += this.pinata;
        BattleManager._instance.OnEnemyDeath += this.brokenPiggybankReward;
        BattleManager._instance.OnEnemyDeath += this.resetUsed;
        BattleManager._instance.OnDeleteCyclingHelmet += this.deleteCyclingHelmet;
        this.Description.text = this.generateDescription();
    }

    public void resetUsed(object sender, eventArgs e)
    {
        if(this == null || this.gameObject == null) return;
        if(this.used)
        {
            this.used = false;
            this.transform.localPosition = Vector3.zero;
            this.transform.localPosition = new Vector3(0f, 0f, 0f);
        }
    }

    [Tooltip("0 for active, 1 for passive")]
    public ItemType itemType;
    [Tooltip("If you can use the item only once per battle")]
    public bool oncePerBattle; //If you can use the card only once per battle
    public bool used;
    [Tooltip("If the item gets destroyed on use")]
    public bool destroyOnUse; //If the item gets destroyed on use

    public Items type; //item name
    public int value;
    [Tooltip("Only select as true when the user is allowed to right click to remove the item")]
    public bool removable; //curse control
    public bool isCurse;

    public int shopValue = 10;

    [Header("TMPro")]
    public TMP_Text title;
    public TMP_Text Description;

    public void OnPointerDown(PointerEventData pointer)
    {
        BattleManager bm = BattleManager._instance;
        if (this.GetComponentInParent<ShopItemContainer>() == null)
        {
            if (pointer.button == PointerEventData.InputButton.Right)
            {
                if (removable)
                {
                    if (isCurse)
                    {
                        bm.modifyCoins(-10);
                        bm.toggleCurse();
                    }
                    if(this.transform.parent.name != "NewItem")
                    {
                        this.unSubscribe();
                        this.GetComponentInParent<ItemContainer>().removeItem();
                        bm.inspect(bm.dc.inspectValue);
                        if(bm.screenState == screen.Shop) bm.updateShopItems();
                    }                    
                }
            }
            if (pointer.button == PointerEventData.InputButton.Left)
            {
                if (this.transform.parent.name == "NewItem")
                {
                    //Add item to player
                    if (!bm.itemManager.isHandFull(true))
                    {
                        bm.addItemToHand(this);
                        bm.toggleScreen(screen.Path);
                    }
                    else
                    {
                        bm.itemManager.text.SetActive(true);
                    }
                }
                else if (bm.screenState == screen.Battle)
                {
                    nerfGun();
                    candyBar();
                    gumballMachine();
                    bandaid();
                }
            }
        }
        else
        {
            if (bm.player.coins >= this.shopValue && !bm.itemManager.isHandFull(true))
            {
                bm.modifyCoins(-this.shopValue);
                this.GetComponentInParent<ShopItemContainer>().reset();
                bm.addItemToHand(this);
                this.removable = !this.removable;
                bm.updateShopItems();
            }
        }
    }

    public bool canBuy()
    {
        BattleManager bm = BattleManager._instance;
        if (bm.player.coins >= this.shopValue && !bm.itemManager.isHandFull(true)) return true;
        return false;
    }

    public void destroyItem()
    {
        this.unSubscribe();
        Destroy(this.gameObject);
    }

    public void unSubscribe()
    {
        BattleManager._instance.OnRoll -= this.bagOfChips;
        BattleManager._instance.OnRoll -= this.looseChange;
        BattleManager._instance.OnRoll -= this.butterFingers;
        BattleManager._instance.OnPlayerDamage -= this.tornPocket;
        BattleManager._instance.OnPlayerDamage -= this.scratchedKnee;
        BattleManager._instance.OnPlayerDamage -= this.brokenPiggybank;
        BattleManager._instance.OnPlayerDamage -= this.pinata;
        BattleManager._instance.OnEnemyDeath -= this.brokenPiggybankReward;
        BattleManager._instance.OnEnemyDeath -= this.resetUsed;
    }

    public bool validate(ItemType iType, Items t)
    {
        if (this.itemType != iType) return false;
        if (this.type != t) return false;
        if (this.oncePerBattle && this.used) return false;
        return true;
    }

    #region Passive Items

    //On roll = 5, heal 1
    public void bagOfChips(object sender, eventArgs e)
    {
        if (!this.validate(ItemType.Passive, Items.BagOfChips)) return;
        foreach (int i in e.individualRolls)
        {
            if (i == 5) {
                BattleManager._instance.ModifyPlayerHP(1);
            }
        }
        this.used = true;
    }

    //On all rolls 6 get rolls * 5coins
    public void looseChange(object sender, eventArgs e)
    {
        if (!this.validate(ItemType.Passive, Items.LooseChange)) return;
        BattleManager bm = sender as BattleManager;
        if (e.individualRolls.Count() < 2) return;
        int res = e.individualRolls.Where(roll => roll == 6).Count();
        if (res == e.individualRolls.Count())
        {
            this.used = true;
            bm.giveReward(new eventArgs { coins = e.individualRolls.Count() * 5 });
        }
    }

    //2x coin rewards from battles, -1 coins on player damage
    public void brokenPiggybank(object sender, eventArgs e)
    {
        if (!this.validate(ItemType.Passive, Items.brokenPiggybank)) return;
        BattleManager bm = sender as BattleManager;
        if (e.damage > 0) return;
        this.used = true;
        bm.modifyCoins(-1);
    }

    public void brokenPiggybankReward(object sender, eventArgs e)
    {
        if (!this.validate(ItemType.Passive, Items.brokenPiggybank)) return;
        Enemy en = sender as Enemy;
        this.used = true;
        BattleManager._instance.modifyCoins(en.coinReward);
    }

    public void deleteCyclingHelmet(object sender, eventArgs e)
    {
        if(!this.validate(ItemType.Passive, Items.CyclingHelmet)) return;
        this.destroyItem();
    }

    #endregion

    #region Active Items

    //Deal 2 damage
    public void nerfGun()
    {
        if (!this.validate(ItemType.Active, Items.NerfGun)) return;
        this.used = true;
        if (this.oncePerBattle) this.transform.localPosition = new Vector3(0f, -5f, 0f);
        BattleManager._instance.ModifyEnemyHP(-2);
        if(this.destroyOnUse) this.destroyItem();
    }

    //heal for 5
    public void bandaid()
    {
        if (!this.validate(ItemType.Active, Items.Bandaid)) return;
        this.used = true;
        if (this.oncePerBattle) this.transform.localPosition = new Vector3(0f, -5f, 0f);
        BattleManager._instance.ModifyPlayerHP(5);
        if(this.destroyOnUse) this.destroyItem();
    }

    //on use +1 to dice rolls, capped at 9
    public void candyBar()
    {
        if (!this.validate(ItemType.Active, Items.CandyBar)) return;
        this.used = true;
        if (this.oncePerBattle) this.transform.localPosition = new Vector3(0f, -5f, 0f);
        BattleManager._instance.plusRolls += 1;
        if(this.destroyOnUse) this.destroyItem();
    }

    //heal 1 hp for 4 coins
    public void gumballMachine()
    {
        if (!this.validate(ItemType.Active, Items.GumballMachine)) return;
        if (BattleManager._instance.player.coins >= 3)
        {
            this.used = true;
            if (this.oncePerBattle) this.transform.localPosition = new Vector3(0f, -5f, 0f);
            BattleManager._instance.modifyCoins(-4);
            BattleManager._instance.ModifyPlayerHP(1);
            if(this.destroyOnUse) this.destroyItem();
        }
    }

    //heal 1 when taking damage
    public void pinata(object sender, eventArgs e)
    {
        if (!this.validate(ItemType.Passive, Items.Pinata)) return;
        if(e.damage < 0)
        {
            BattleManager._instance.ModifyPlayerHP(1);
            if(this.destroyOnUse) this.destroyItem();
        }
    }

    #endregion

    #region Cursed Items

    //lose 2 coins on player damage
    public void tornPocket(object sender, eventArgs e)
    {
        if (!this.validate(ItemType.Cursed, Items.TornPocket)) return;
        BattleManager._instance.modifyCoins(-2);
    }

    //lose 1 HP on player damage
    public void scratchedKnee(object sender, eventArgs e)
    {
        if (!this.validate(ItemType.Cursed, Items.ScratchedKnee)) return;
        BattleManager._instance.ModifyPlayerHP(-1);
    }

    //heal enemy for each 1 you roll
    public void butterFingers(object sender, eventArgs e)
    {
        if (!this.validate(ItemType.Cursed, Items.ButterFingers)) return;
        foreach (int roll in e.individualRolls)
        {
            if (roll == 1) BattleManager._instance.ModifyEnemyHP(1);
        }
    }

    #endregion

    public string generateDescription()
    {
        //Generate string for description here
        switch (this.type)
        {
            case Items.brokenPiggybank:
                this.title.text = "Broken piggybank";
                return $"{this.getConditional()}Lose 1 coin on damage. Gain double the rewards of battle.";
            case Items.LooseChange:
                this.title.text = "Loose Change";
                return $"{this.getConditional()}Get 5 coins per dice every time all of them roll 6 (Minimum 2 die).";
            case Items.BagOfChips:
                this.title.text = "Bag of chips";
                return $"{this.getConditional()}Heal 1 HP for each 5 you roll.";
            case Items.NerfGun:
                this.title.text = "Nerf gun";
                return $"{this.getConditional()}Do 2 damage to the enemy.";
            case Items.CandyBar:
                this.title.text = "Candy bar";
                return $"{this.getConditional()}Add +1 to all individual rolls (max 9).";
            case Items.GumballMachine:
                this.title.text = "Gumball machine";
                return $"{this.getConditional()}Spend 3 coins to heal 1 HP.";
            case Items.Bandaid:
                this.title.text = "Band-aid";
                return $"{this.getConditional()}Heal 5.";
            case Items.BigStick:
                this.title.text = "Big stick";
                return $"{this.getConditional()}+1 damage";
            case Items.Pinata:
                this.title.text = "Pinata";
                return $"{this.getConditional()}Heal 1 every time you get hit.";
            case Items.TornPocket:
                this.title.text = "Torn pocket";
                return $"{this.getConditional()}Lose 2 gold when you get hit.";
            case Items.ScratchedKnee:
                this.title.text = "Scratched Knee";
                return $"{this.getConditional()}Lose an additional 1 health when hit by an enemy.";
            case Items.Chores:
                this.title.text = "Chores";
                return "";
            case Items.StrangerDanger:
                this.title.text = "Stranger Danger";
                return $"{this.getConditional()}Increase elite monster chance by 5%.";
            case Items.RustySword:
                this.title.text = "Rusty Sword";
                return $"{this.getConditional()}Increases minimum roll for enemies by 1.";
            case Items.ButterFingers:
                this.title.text = "Butter Fingers";
                return $"{this.getConditional()}Heal enemy for 1 HP for each roll of 1.";
            case Items.CyclingHelmet:
                this.title.text = "Cycling Helmet";
                return $"On fatal damage, destroy item and set your HP to 1.";
            default:
                return "";
        }
    }

    public string getConditional()
    {
        if (this.destroyOnUse) return "Destroy this. ";
        if (this.oncePerBattle) return "Once per battle. ";
        return "";
    }
}

public enum Items
{
    empty,

    BagOfChips, //heal 1 when you roll 5
    NerfGun, //do 1 damage
    LooseChange,//On all rolls 6 get rolls*10 coins - min 2 die
    CandyBar, //add +1 to individual rolls, capped at 6
    brokenPiggybank, //2x coin rewards from battles, -1 coins on player damage
    GumballMachine, //Once per battle, heal 1 hp for 3 coins
    Bandaid, //One time use: heal 5
    BigStick, //+1 to player damage
    Pinata, //Gain 1 gold for everytime you get hit

    //Curses
    TornPocket, //Lose 2 gold when you get hit
    ScratchedKnee, //Lose +1 health when hit by an enemy
    Chores, //Does nothing, just take up a spot in hand
    StrangerDanger, //Increase elite monster chance by 5%
    RustySword,  //Increases min-roll by 1 for enemies
    ButterFingers, //Every roll of 1 heals the enemy for 1

    //Event specific
    CyclingHelmet //If you are about to die, consume item and set HP to 1
}