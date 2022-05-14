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

        BattleManager._instance.debugKillEnemy += checkEnemy;
        BattleManager._instance.debugSetEnemyHP += checkEnemy;
    }

    public List<GameObject> allEnemies = new List<GameObject>();

    public GameObject currentEnemy;
    public const float baseDropChance = 0f;
    public float dropChance;
    
    public void calculateDropChance(object sender, eventArgs e)
    {
        Enemy enemy = currentEnemy.GetComponent<Enemy>();
        if(enemy.itemReward) return;
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
        if(e.debug_string != null)
        {
            if(this.currentEnemy != null) BattleManager._instance.destroyEnemy(this.currentEnemy.GetComponent<Enemy>());
            List<GameObject> enemies = this.allEnemies.Where(enemy => bm.compareStrings(enemy.GetComponent<Enemy>().enemyName.Replace(" ", ""), e.debug_string)).ToList();
            if(enemies.Count > 0) {
                currentEnemy = Instantiate(enemies[0]);
            }
            else
            {
                BattleManager._instance.message("Enemy not found");
                return;
            }
        }
        else
        {
            int level = 1;
            if(bm.level <= bm.easyRange) level = 1;
            if(bm.level <= bm.mediumRange && bm.level > bm.easyRange) level = 2;
            if(bm.level <= bm.hardRange && bm.level > bm.mediumRange) level = 3;
            if(e.isElite) level = 4;
            List<GameObject> enemies = this.allEnemies.Where(enemy => enemy.GetComponent<Enemy>().difficultyValue == level ).ToList();
            currentEnemy = Instantiate(enemies[Random.Range(0, enemies.Count())]);
        }
        
        if(this.currentEnemy != null) 
        {
            this.currentEnemy.transform.SetParent(this.transform, false);
            this.currentEnemy.GetComponent<Enemy>().subscribe();
            BattleManager._instance.toggleScreen(screen.Battle);
        }
    }

    public void checkEnemy(object sender, eventArgs e)
    {
        if(currentEnemy == null)
        {
            BattleManager._instance.message("No enemy found");
        }
    }

    public void toggle(object sender, eventArgs e)
    {
        if(e.screenValue == screen.GameOver)
        {
            this.gameObject.SetActive(false);
        }
    }

}