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

    List<int> weights = new List<int>() { 5, 25, 10 };

    // Start is called before the first frame update
    void Start()
    {
        BattleManager._instance.OnToggleScreen += this.toggle;
    }

    public void generatePaths()
    {
        //Get the amount of paths
        int amount = this.getAmountOfPaths();
        int max = 4;
        for (int i = 0; i < amount; i++)
        {
            switch (Random.Range(1, max))
            {
                case 1: //monster
                    if (Random.Range(0, 100) < 20)
                    {
                        //elite
                        this.generatePrefab(elitePrefab, this.containers[i].transform);
                    }
                    else
                    {
                        //normal
                        this.generatePrefab(monsterPrefab, this.containers[i].transform);
                    }
                    break;
                case 2: //event
                    this.generatePrefab(eventPrefab, this.containers[i].transform);
                    break;
                case 3: //shop
                    this.generatePrefab(shopPrefab, this.containers[i].transform);
                    max--;
                    break;
            }
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
