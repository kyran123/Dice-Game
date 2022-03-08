using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceManager : MonoBehaviour
{   
    public List<Die> dice;
    public List<Rigidbody> diceRB;

    void Update() {
        for(int index = 0; index < dice.Count; index++) {
            if(dice[index].logged != true) {
                if(diceRB[index].IsSleeping()) {
                Debug.Log(dice[index].getSide());
                    dice[index].logged = true;
                }
            }
            if(Input.GetKeyDown(KeyCode.Space)) {
                dice[index].logged = false;
                diceRB[index].AddForce(new Vector3(0f, 500f, 0f), ForceMode.Force);
                diceRB[index].AddTorque(new Vector3(
                    Random.Range(-75f, 75f),
                    Random.Range(-75f, 75f),
                    Random.Range(-75f, 75f)
                ));
            }
        }
    }
}
