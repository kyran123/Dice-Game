using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PathManager : MonoBehaviour
{
    public List<GameObject> containers = new List<GameObject>();

    public GameObject elitePrefab;
    public GameObject monsterPrefab;
    public GameObject eventPrefab;
    public GameObject shopPrefab;

    [SerializeField]
    private bool hadShop = false;

    List<int> weights = new List<int>() { 6, 24, 20 }; // 12% 1 path, 48% 2 paths, 40% 3 paths

    public float eliteChance = 0.5f;
    public int eliteChanceCap = 20;
    public int EliteChance {
        get {
            int chance = (int)(BattleManager._instance.level * eliteChance);
            if(chance < eliteChanceCap) return chance;
            else return eliteChanceCap;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        BattleManager._instance.OnToggleScreen += this.toggle;
        BattleManager._instance.OnClearPaths += this.clearPaths;
    }

    public int shopChanceBoost() 
    {
        BattleManager bm = BattleManager._instance;
        if(this.hadShop) return 0;
        if(bm.level == 5) return 25;
        if(bm.level == 12) return 25;
        return 0;
    }

    public void generatePaths()
    {
        int elite = EliteChance + BattleManager._instance.itemManager.getItemsValue(Items.StrangerDanger);
        //Get the amount of paths
        int amount = this.getAmountOfPaths();
        int max = 100;
        if(hadShop) 
        {
            max -= 10;
            this.hadShop = false;
        }
        for (int i = 0; i < amount; i++)
        {
            int value = Random.Range(1, max + this.shopChanceBoost());
            if(value <= 75) 
            {
                if (Random.Range(0, 100) < EliteChance)
                {
                    //elite
                    this.generatePrefab(elitePrefab, this.containers[i].transform);
                }
                else
                {
                    //normal
                    this.generatePrefab(monsterPrefab, this.containers[i].transform);
                }
            }
            if(value > 75 && value <= 90)
            {
                //Event
                this.generatePrefab(eventPrefab, this.containers[i].transform);
                break;
            }
            if(value > 90)
            {
                //Shop
                this.generatePrefab(shopPrefab, this.containers[i].transform);
                max -= 10;
                this.hadShop = true;
                break;
            }
        }
    }

    public void clearPaths(object sender, eventArgs e)
    {
        foreach(GameObject container in this.containers)
        {
            if(container.transform.childCount > 0) Destroy(container.GetComponentInChildren<Path>().gameObject);
        }
    }

    public void generatePrefab(GameObject prefab, Transform parent)
    {
        GameObject obj = Instantiate(prefab);
        obj.transform.SetParent(parent, false);
    }

    public int getAmountOfPaths()
    {
        int total = 0;
        int totalWeight = this.weights.Sum();
        int random = Random.Range(0, totalWeight);
        for (int index = 0; index < this.weights.Count(); index++)
        {
            total += this.weights[index];
            if (total >= random) return index + 1;
        }
        return 1;
    }

    public void toggle(object sender, eventArgs e)
    {
        if (e.screenValue == screen.Path)
        {
            this.generatePaths();
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
        }
        else
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        }
    }

}
