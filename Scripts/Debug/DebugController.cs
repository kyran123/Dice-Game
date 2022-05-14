using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Text;
using System.Linq;
using TMPro;

public class DebugController : MonoBehaviour
{
    public GameControls debugControls;

    bool showConsole;

    string input;
    public GameObject inspectGO;
    public string inspectValue;
    public TMP_Text inspectInfo;
    string msg;

    public List<object> commandList = new List<object>() {
        new DebugCommand<int>("modify_player_hp", "Changes the player by x", "modify_player_hp <x>", typeof(int), (x) => { BattleManager._instance.ModifyPlayerHP(x); }),
        new DebugCommand<int>("set_player_hp", "Sets the player hp to x", "set_player_hp <x>", typeof(int), (x) => { BattleManager._instance.setPlayerHP(x); }),
        new DebugCommand<int>("modify_coins", "Changes coins by x", "modify_coins <x>", typeof(int), (x) => { BattleManager._instance.modifyCoins(x); }),
        new DebugCommand<int>("set_coins", "Sets coins to x", "set_coins <x>", typeof(int), (x) => { BattleManager._instance.setCoins(x); }),
        new DebugCommand<int>("set_damage", "Sets damage to x", "set_damage <x>", typeof(int), (x) => { BattleManager._instance.setDamage(x); }),

        new DebugCommand<string>("spawn_enemy", "Spawn a specific enemy", "spawn_enemy <x>", typeof(string), (x) => { BattleManager._instance.spawnEnemy(x); }),
        new DebugCommand<int>("modify_enemy_hp", "Changes enemy hp by x", "modify_enemy_hp <x>", typeof(int), (x) => { BattleManager._instance.ModifyEnemyHP(x); }),
        new DebugCommand<int>("set_enemy_hp", "Sets the enemy hp to x", "set_enemy_hp <x>", typeof(int), (x) => { BattleManager._instance.setEnemyHP(x); }),
        new DebugCommand<int>("set_enemy_damage", "Set the enemy damage value", "set_enemy_damage <x>", typeof(int), (x) => { BattleManager._instance.setEnemyDamage(x); }),
        new DebugCommand<int>("set_enemy_roll", "Set the minimum roll for enemy", "set_enemy_roll <x>", typeof(int), (x) => { BattleManager._instance.setEnemyRoll(x); }),
        new DebugCommand("kill_enemy", "Kills the current enemy on the board", "kill_enemy", null, () => { BattleManager._instance.KillEnemy(); }),

        new DebugCommand<string>("add_item", "Adds item to hand", "add_item <x>", typeof(string), (x) => { BattleManager._instance.addItem(x); }),
        
        new DebugCommand<string>("event", "Spawns the event x", "event <x>", typeof(string), (x) => { BattleManager._instance.toggleScreen(screen.Event, x); }),

        new DebugCommand<string>("shop", "Spawns a shop", "shop x", typeof(string), (x) => { BattleManager._instance.toggleScreen(screen.Shop); }),

        new DebugCommand<List<int>>("add_die", "Adds a die", "add_die [x,x,x,x,x,x] (Optional)", typeof(List<int>), (x) => { BattleManager._instance.addDie(x); }),

        new DebugCommand<string>("inspect", "Show info for an element", "inspect ,x.", typeof(string), (x) => { BattleManager._instance.inspect(x); }),
    };

    #region Not important stuff

    void OnEnable()
    {
        this.debugControls.Enable();
    }

    void OnDisable()
    {
        this.debugControls.Disable();
    }

    void Awake()
    {
        debugControls = new GameControls();
    }

    #endregion

    void Start()
    {
        debugControls.Debug.ToggleDebug.performed += OnToggleConsole;
        debugControls.Debug.Return.performed += OnReturn;
        BattleManager._instance.debugMessage += logMessage;
        BattleManager._instance.debugInspect += setInspectvalues;
    }

    public void setInspectvalues(object sender, eventArgs e)
    {
        this.inspectGO.SetActive(true);
        if(e.debug_enemy != null)
        {
            this.inspectValue = "enemy";
            Enemy enemy = e.debug_enemy;
            this.inspectInfo.text = $"{enemy.enemyName} - Minroll: <#ddd>{enemy.minRoll}</color> / HP: <#ddd>{enemy.HP}</color> / Damage <#ddd>{enemy.damage}</color> \n";
            this.inspectInfo.text += $"Coin reward: <#ddd>{enemy.coinReward}</color> and item reward is <#ddd>{enemy.itemReward}</color> \n \n";
            foreach(EnemySkills skill in enemy.getEnemySkills())
            {
                string value = "";
                if(skill.diceRolls.Count() > 0) 
                {
                    value += $"[";
                    foreach(int roll in skill.diceRolls)
                    {
                        value += $"{roll}, ";
                    }
                    value += $"]";
                }
                else value = skill.value.ToString();
                string iEffect = "";
                if(skill.effect == effect.item) iEffect += skill.iEffect;
                else if(skill.effectValue == 0) iEffect += $"Min: {skill.min} to Max: {skill.max}";
                else iEffect = skill.effectValue.ToString();
                this.inspectInfo.text += $"Conditions: {skill.conditions} \n Value: {value} {skill.op.ToString()} \n Once per battle: {skill.oncePerBattle} \n Effects: {skill.effect} {iEffect} \n \n";
            }
        }
        if(e.debug_player != null)
        {
            this.inspectValue = "player";
            Player player = e.debug_player;
            this.inspectInfo.text = $"Player HP: <#ddd>{player.HP}</color> / Damage: <#ddd>{player.damage}</color> / Coins: <#ddd>{player.coins}</color> \n \n";
            foreach(Item item in e.debug_items)
            {
                this.inspectInfo.text += $" Item: <#ddd>{item.type}</color> \n Type: <#ddd>{item.itemType}</color> \n Once per battle: <#ddd>{item.oncePerBattle}</color> Is used on turn: <#ddd>{item.used}</color> \n Destroy on use: <#ddd>{item.destroyOnUse}</color> \n Is curse: <#ddd>{item.isCurse}</color> removable: <#ddd>{item.removable}</color> \n ";
                this.inspectInfo.text += $"Effect: <#ddd>{item.Description.text}</color> value <#ddd>{item.value}</color> \n Shop cost: <#ddd>{item.shopValue}</color> \n \n";
            }
        }
        if(e.debug_string != null) 
        {
            this.inspectValue = "";
            this.inspectGO.SetActive(false);
        }
    }

    public void logMessage(object sender, eventArgs e)
    {
        this.msg = e.debug_string;
    }

    public void OnToggleConsole(InputAction.CallbackContext context)
    {
        this.showConsole = !this.showConsole;
    }

    public void OnReturn(InputAction.CallbackContext context)
    {
        this.HandleInput();
    }

    private void OnGUI()
    {
        if (!showConsole) return;

        float y = 0;

        GUI.Box(new Rect(0, y, Screen.width, 30), "");
        GUI.backgroundColor = new Color(0f, 0f, 0f, 1f);

        input = GUI.TextField(new Rect(0f, y + 2.5f, Screen.width, 25f), input);

        GUI.skin.label.alignment = TextAnchor.MiddleRight;
        GUI.color = Color.red;
        GUI.Label(new Rect(Screen.width - 520f, 2.5f, 500f, 20f), msg);

    }

    private void HandleInput()
    {
        if(input.Count(Char.IsWhiteSpace) > 1) 
        {
            this.msg = "Invalid input";
            return;
        }
        string[] properties = input.Split(' ');

        foreach(var command in commandList)
        {
            DebugCommandBase dcBase = (command as DebugCommandBase);
            string a = new StringBuilder(properties[0].ToLower()).ToString();
            string b = new StringBuilder(dcBase.commandId.ToLower()).ToString();
            if(string.Equals(a, b))
            {
                this.msg = "";
                this.input = "";
                if(dcBase.commandType == null) (command as DebugCommand).Invoke();
                if(dcBase.commandType == typeof(int)) {
                    try { (command as DebugCommand<int>).Invoke(int.Parse(properties[1])); break; }
                    catch(InvalidCastException e) { Debug.LogError(e); break; }
                }
                if(dcBase.commandType == typeof(string)) {
                    try 
                    { 
                        if(properties.Count() > 1) (command as DebugCommand<string>).Invoke(properties[1]); 
                        else (command as DebugCommand<string>).Invoke("");
                        break;
                    }
                    catch(InvalidCastException e) { Debug.LogError(e); break; }
                }
                if(dcBase.commandType == typeof(bool)) {
                    try { (command as DebugCommand<bool>).Invoke(bool.Parse(properties[1])); break; }
                    catch(InvalidCastException e) { Debug.LogError(e); }
                }
                if(dcBase.commandType == typeof(List<int>)) {
                    try {
                        List<int> newInts = new List<int>();
                        if(properties.Count() > 1) 
                        {
                            properties[1] = properties[1].Replace("[", "");
                            properties[1] = properties[1].Replace("]", "");
                            foreach(string value in properties[1].Split(','))
                            {
                                newInts.Add(int.Parse(value));
                            }
                        }
                        (command as DebugCommand<List<int>>).Invoke(newInts);
                        break;
                    }
                    catch(InvalidCastException e) { Debug.LogError(e); break; }
                }
                if(dcBase.commandType == typeof(List<string>)) {
                    try { 
                        List<string> newStrings = new List<string>();
                        if(properties.Count() > 1) 
                        {
                            properties[1] = properties[1].Replace("[", "");
                            properties[1] = properties[1].Replace("]", "");
                            foreach(string value in properties[1].Split(','))
                            {
                                newStrings.Add(value);
                            }
                        }
                        (command as DebugCommand<List<string>>).Invoke(newStrings);
                        break;
                    }
                    catch(InvalidCastException e) { Debug.LogError(e); break; }
                }
            }
            else
            {
                this.msg = "Command not found";
            }
        }
    }

}
