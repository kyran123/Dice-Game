using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DiceManager : MonoBehaviour
{
    //6 sided die
    public GameObject die6Prefab;
    /*
        10 - 1 damage
        11 - 2 damage
        12 - heal 1
        13 - heal 2
        14 - take 1 damage
        15 - take 2 damage
        16 - get 1 coin
        17 - get 2 coins
        18 - lose 1 coin
        19 - lose 2 coins
    */

    public List<List<int>> die6Sides = new List<List<int>>() {
        new List<int> { 9, 0, 0, 1, 0, 9 },
        new List<int> { 12, 0, 13, 12, 13, 0 },
        new List<int> { 19, 0, 4, 5, 17, 6 },
        new List<int> { 0, 1, 2, 2, 3, 5 },
        new List<int> { 0, 1, 2, 4, 5, 6 },
        new List<int> { 14, 3, 3, 4, 4, 6 },
        new List<int> { 0, 0, 3, 3, 6, 6 }, //Below Average ^
        new List<int> { 1, 2, 3, 4, 5, 6 }, //Average (sum is 21)
        new List<int> { 0, 0, 3, 3, 6, 6 }, //Above Average v
        new List<int> { 3, 3, 4, 11, 10, 6 },
        new List<int> { 2, 16, 17, 2, 8, 9 },
        new List<int> { 10, 11, 4, 6, 8, 11 },
        new List<int> { 0, 1, 3, 5, 7, 9 },
        new List<int> { 17, 0, 2, 4, 6, 8 },
        new List<int> { 10, 10, 12, 12, 10, 12 }
    };

    public GameObject dieBox;

    public void addDie(object sender, eventArgs e)
    {
        List<int> sides;
        if (e.individualRolls.Count() != 6) sides = this.die6Sides[Random.Range(0, this.die6Sides.Count)];
        else sides = e.individualRolls;
        GameObject newDie = Instantiate(die6Prefab);
        newDie.transform.SetParent(this.dieBox.transform, false);
        newDie.transform.position = new Vector3(0, 10, 0);
        List<SideValue> allSides = new List<SideValue>();
        sides.ForEach((side) =>
        {
            allSides.Add((SideValue)side);
        });
        newDie.GetComponent<Die>().updateSides(allSides);
        this.dice.Add(newDie.GetComponent<Die>());
    }

    public bool canBuyDie()
    {
        if (this.dice.Count() < 4) return true;
        return false;
    }

    public bool canRemovedie()
    {
        if (this.dice.Count() > 1) return true;
        return false;
    }

    public int dieCount()
    {
        return this.dice.Count();
    }

    public void removeDie(object sender, eventArgs e)
    {
        Die die = e.die;
        if (die == null) die = this.dice[Random.Range(0, this.dice.Count() - 1)];
        this.dice.Remove(this.dice.Where(d => d.GetInstanceID() == die.GetInstanceID()).ToList().First());
        Destroy(die.gameObject);
    }

    public List<Die> dice; //Max 4 dice
    private bool cleared = true;

    public List<int> diceResults = new List<int>();

    void Start()
    {
        this.addDie(this, new eventArgs { individualRolls = new List<int>() { 1, 2, 3, 4, 5, 6 } });
        BattleManager._instance.OnAddDie += this.addDie;
        BattleManager._instance.OnRemoveDie += this.removeDie;
        BattleManager._instance.OnEnemyDeath += this.onEnemyDeath;
        BattleManager._instance.OnBattle += this.battleStart;
    }

    public void onEnemyDeath(object sender, eventArgs e)
    {
        foreach(Die die in dice)
        {
            die.logged = true;
        }
        cleared = true;
    }

    public void battleStart(object sender, eventArgs e)
    {
        foreach(Die die in dice)
        {
            die.logged = true;
        }
    }

    void Update()
    {
        if (this.diceResults.Count >= this.dice.Count)
        {
            BattleManager._instance.OnDiceRoll(this.diceResults);
            this.diceResults.Clear();
            cleared = true;
        }
        if (this.dice.Count > 0)
        {
            for (int index = 0; index < dice.Count; index++)
            {
                if (dice[index].logged != true)
                {
                    if (dice[index].GetComponent<Rigidbody>().IsSleeping())
                    {
                        diceResults.Add(dice[index].getSide());
                        dice[index].logged = true;
                    }
                }
                if (Input.GetKeyDown(KeyCode.Space) && cleared && !BattleManager._instance.nItemManager.hasItem() && BattleManager._instance.screenState == screen.Battle)
                {
                    if (index == dice.Count - 1) cleared = false;
                    dice[index].logged = false;
                    dice[index].transform.position = new Vector3(20f, 10f, -50f);
                    dice[index].GetComponent<Rigidbody>().AddForce(new Vector3(
                        Random.Range(-300f, -100f),
                        Random.Range(-100f, 100f),
                        Random.Range(1800f, 2000f)
                    ), ForceMode.Force);
                    dice[index].GetComponent<Rigidbody>().AddTorque(new Vector3(
                        Random.Range(-350f, 350f),
                        Random.Range(-350f, 350f),
                        Random.Range(-350f, 350f)
                    ));
                }
            }
        }
    }
}
