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
        return Instantiate(allEvents[Random.Range(0, allEvents.Count)]);
    }

    public void setEvent(GameObject eventPrefab)
    {
        this.destroyCurrentEvent();
        Instantiate(eventPrefab).transform.SetParent(this.transform, false);
    }

    public void unsubscribeDecisions() 
    {
        List<Decision> decisions = this.GetComponentsInChildren<Decision>().ToList();
        foreach(Decision dec in decisions)
        {
            BattleManager._instance.OnHandIsFull -= dec.enableDecision;
        }
    }

    public void destroyCurrentEvent()
    {
        foreach (Transform child in this.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void toggle(object sender, eventArgs e)
    {
        BattleManager bm = sender as BattleManager;
        this.destroyCurrentEvent();
        if (e.startEvent)
        {
            GameObject sEvent = Instantiate(startingEvent);
            sEvent.transform.SetParent(this.transform, false);
            List<Decision> decisions = sEvent.GetComponentsInChildren<Decision>().ToList();
            GameObject descriptionObject = sEvent.transform.Find("Description").gameObject;
            descriptionObject.GetComponent<TMPro.TextMeshProUGUI>().text = "Choose wisely";
            foreach (Decision decision in decisions)
            {
                decision.gameObject.SetActive(true);
                startingDecision newDecision = this.startingDecisions[Random.Range(0, this.startingDecisions.Count())];
                decision.title.text = newDecision.Text;
                decision.isReward = true;
                decision.reward = newDecision.reward;
                this.startingDecisions.Remove(newDecision);
            }
        }
        else
        {
            if (e.screenValue == screen.Event)
            {
                if(e.debug_string != null)
                {
                    List<GameObject> eventGO = this.allEvents.Where(i => BattleManager._instance.compareStrings(i.GetComponent<Event>().eventName.Replace(" ", ""), e.debug_string)).ToList();
                    if(eventGO.Count() > 0) Instantiate(eventGO[0]).transform.SetParent(this.transform, false);
                    else BattleManager._instance.message("No event found");
                }
                else
                {
                    generateEvent().transform.SetParent(this.transform, false);
                }
            }
        }

    }
}
