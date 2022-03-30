using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    void Start()
    {
        BattleManager._instance.OnToggleScreen += this.toggle;
    }

    public List<GameObject> allEvents = new List<GameObject>();

    public GameObject generateEvent()
    {
        return Instantiate(allEvents[Random.Range(0, allEvents.Count - 1)]);
    }

    public void toggle(object sender, eventArgs e)
    {
        BattleManager bm = sender as BattleManager;
        if(e.screenValue == screen.Event)
        {
            generateEvent().transform.SetParent(this.transform, false);
        }
    }
}
