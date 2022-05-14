using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class updateLevel : MonoBehaviour
{
    public TMP_Text text;
    void Start()
    {
        BattleManager._instance.OnEnemyDeath += show;
    }

    public void show(object sender, eventArgs e)
    {
        this.text.text = BattleManager._instance.level.ToString();
    }
}
