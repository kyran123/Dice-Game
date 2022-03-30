using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyManager : MonoBehaviour
{
    void Start()
    {
        BattleManager._instance.OnGenerateEnemy += generateNewEnemy;
    }

    public List<GameObject> allEnemies = new List<GameObject>();

    public GameObject currentEnemy;

    public void generateNewEnemy(object sender, eventArgs e)
    {
        BattleManager bm = BattleManager._instance;
        int level = 1;
        if(bm.level <= bm.easyRange) level = 1;
        if(bm.level <= bm.mediumRange) level = 2;
        if(bm.level <= bm.hardRange) level = 3;
        List<GameObject> enemies = this.allEnemies.Where(enemy => enemy.GetComponent<Enemy>().difficultyValue == level ).ToList();
        currentEnemy = Instantiate(enemies[Random.Range(0, enemies.Count() - 1)]);
        this.currentEnemy.transform.SetParent(this.transform, false);
    }

}