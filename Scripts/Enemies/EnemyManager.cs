using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyManager : MonoBehaviour
{
    void Start()
    {
        BattleManager._instance.OnGenerateEnemy += generateNewEnemy;
        BattleManager._instance.OnEnemyDeath += calculateDropChance;
        dropChance = baseDropChance;
    }

    public List<GameObject> allEnemies = new List<GameObject>();

    public GameObject currentEnemy;
    public const float baseDropChance = 15f;
    public float dropChance;
    
    public void calculateDropChance(object sender, eventArgs e)
    {
        Enemy enemy = currentEnemy.GetComponent<Enemy>();
        switch(enemy.difficultyValue)
        {
            case 1:
                dropChance += 5f;
                break;
            case 2:
                dropChance += 10f;
                break;
            case 3:
                dropChance += 15f;
                break;
            case 4:
                dropChance = 100f;
                break;
        }
        enemy.itemReward = dropChance >= Random.Range(0,101);
        if(enemy.itemReward) dropChance = baseDropChance;
    }

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