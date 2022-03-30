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
       new List<int> { 1, 1, 2, 2, 3, 4 },
       new List<int> { 1, 1, 2, 2, 3, 4 },
       new List<int> { 1, 1, 2, 2, 3, 4 },
       new List<int> { 1, 1, 2, 2, 3, 4 },
       new List<int> { 1, 1, 2, 2, 3, 4 },
       new List<int> { 1, 1, 2, 2, 3, 4 },
       new List<int> { 1, 1, 2, 2, 3, 4 },
       new List<int> { 1, 1, 2, 2, 3, 4 },
       new List<int> { 1, 1, 2, 2, 3, 4 },
       new List<int> { 1, 2, 3, 4, 5, 6 },
    };

    public GameObject dieBox;

    public void addDie(object sender, eventArgs e)
    {
        List<int> sides;
        if (e.individualRolls == null) sides = this.die6Sides[Random.Range(0, this.die6Sides.Count)];
        else sides = e.individualRolls;
        GameObject newDie = Instantiate(die6Prefab);
        newDie.transform.SetParent(this.dieBox.transform, false);
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

    public void removeDie(object sender, eventArgs e)
    {
        Debug.Log("remove die");
        Die die = e.die;
        if (die == null) die = this.dice[Random.Range(0, this.dice.Count() - 1)];
        Debug.Log($"Die: {die}");
        this.dice.Remove(this.dice.Where(d => d.GetInstanceID() == die.GetInstanceID()).ToList().First());
        Destroy(die.gameObject);
    }

    public List<Die> dice; //Max 4 dice
    private bool cleared = true;

    public List<int> diceResults = new List<int>();

    void Start()
    {
        this.addDie(null, new eventArgs {});
        this.addDie(null, new eventArgs {});
        BattleManager._instance.OnAddDie += this.addDie;
        BattleManager._instance.OnRemoveDie += this.removeDie;
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
                        dice[index].logged = true;
                        diceResults.Add(dice[index].getSide());
                    }
                }
                if (Input.GetKeyDown(KeyCode.Space) && cleared)
                {
                    if (index == dice.Count - 1) cleared = false;
                    dice[index].logged = false;
                    dice[index].GetComponent<Rigidbody>().AddForce(new Vector3(
                        Random.Range(-100f, 100f),
                        1000f,
                        Random.Range(-100f, 100f)
                    ), ForceMode.Force);
                    dice[index].GetComponent<Rigidbody>().AddTorque(new Vector3(
                        Random.Range(-275f, 275f),
                        Random.Range(-275f, 275f),
                        Random.Range(-275f, 275f)
                    ));
                }
            }
        }
    }
}
