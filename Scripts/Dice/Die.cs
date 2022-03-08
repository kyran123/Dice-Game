using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Die : MonoBehaviour {
    
    public List<Transform> sides = new List<Transform>();
    public bool logged = true;

    public int getSide() {
        float highestSide = 0;
        int value = 0;
        foreach(Transform side in this.sides) {
            if(side.position.y > highestSide) {
                highestSide = side.position.y;
                value = int.Parse(side.name);
            }
        }
        return value;
    }

}
