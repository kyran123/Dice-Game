using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EventManager : MonoBehaviour
{
    void Start()
    {
        BattleManager._instance.OnToggleScreen += this.toggle;
    }

    public GameObject startingEvent;
    public List<startingDecision> startingDecisions = new List<startingDecision>();

    public List<GameObject> allEvents = new List<GameObject>();

    public GameObject generateEvent()
    {
        return Instantiate(allEvents[Random.Range(0, allEvents.Count - 1)]);
    }

    public void toggle(object sender, eventArgs e)
    {
        BattleManager bm = sender as BattleManager;
        if (e.startEvent)
        {
            GameObject sEvent = Instantiate(startingEvent);
            sEvent.transform.SetParent(this.transform, false);
            List<Decision> decisions = sEvent.GetComponentsInChildren<Decision>().ToList();
            foreach(Decision decision in decisions)
            {
                startingDecision newDecision = this.startingDecisions[Random.Range(0, this.startingDecisions.Count() - 1)];
                decision.title.text = newDecision.Text;
                decision.reward = newDecision.reward;
                this.startingDecisions.Remove(newDecision);
            }
        }
        else
        {
            if (e.screenValue == screen.Event)
            {
                generateEvent().transform.SetParent(this.transform, false);
            }
        }

    }
}
