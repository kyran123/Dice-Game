using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceManager : MonoBehaviour
{   
    public List<Die> dice;
    public List<Rigidbody> diceRB;
    private bool cleared = true;

    public List<int> diceResults = new List<int>();

    void Update() {
        if(this.diceResults.Count >= this.dice.Count) {
            BattleManager._instance.OnDiceRoll(this.diceResults);
            this.diceResults.Clear();
            cleared = true;
        }
        for(int index = 0; index < dice.Count; index++) {
            if(dice[index].logged != true) {
                if(diceRB[index].IsSleeping()) {
                    dice[index].logged = true;
                    diceResults.Add(dice[index].getSide());
                }
            }
            if(Input.GetKeyDown(KeyCode.Space) && cleared) {
                if(index == dice.Count - 1) cleared = false;
                dice[index].logged = false;
                diceRB[index].AddForce(new Vector3(
                    Random.Range(-100f, 100f), 
                    1000f, 
                    Random.Range(-100f, 100f)
                ), ForceMode.Force);
                diceRB[index].AddTorque(new Vector3(
                    Random.Range(-275f, 275f),
                    Random.Range(-275f, 275f),
                    Random.Range(-275f, 275f)
                ));
            }
        }
    }
}
