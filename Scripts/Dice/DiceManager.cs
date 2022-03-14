using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceManager : MonoBehaviour
{   
    public List<Die> dice;
    public List<Rigidbody> diceRB;

    public List<int> diceResults = new List<int>();

    void Update() {
        if(this.diceResults.Count >= 2) {
            BattleManager._instance.attack(this.diceResults);
            this.diceResults.Clear();
        }
        for(int index = 0; index < dice.Count; index++) {
            if(dice[index].logged != true) {
                if(diceRB[index].IsSleeping()) {
                    Debug.Log(dice[index].getSide());
                    dice[index].logged = true;
                    diceResults.Add(dice[index].getSide());
                }
            }
            if(Input.GetKeyDown(KeyCode.Space)) {
                dice[index].logged = false;
                diceRB[index].AddForce(new Vector3(
                    Random.Range(-100f, 100f), 
                    700f, 
                    Random.Range(-100f, 100f)
                ), ForceMode.Force);
                diceRB[index].AddTorque(new Vector3(
                    Random.Range(-175f, 175f),
                    Random.Range(-175f, 175f),
                    Random.Range(-175f, 175f)
                ));
            }
        }
    }
}
