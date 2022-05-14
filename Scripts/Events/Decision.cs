using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Linq;

public class Decision : MonoBehaviour, IPointerDownHandler
{
    /*
        What can a decision do
        modify player HP
        modify player Coins
        add/remove items
        add/remove die
    */

    [SerializeField, Space(5)]
    public List<DecisionCondition> conditions = new List<DecisionCondition>();

    [SerializeField, Space(5)]
    public bool isReward = false;
    public Reward reward = new Reward();

    [Space(15)]
    public List<Response> responses;

    public TMP_Text title;

    private bool isEnabled = false;

    void Start()
    {
        this.enableDecision(this, new eventArgs { });
        BattleManager._instance.OnHandIsFull += this.enableDecision;
    }

    public bool checkConditions()
    {
        if (this.conditions.Count < 1) return true;
        foreach (DecisionCondition condition in this.conditions)
        {
            if (!condition.validate()) return false;
        }
        return true;
    }

    public void enableDecision(object sender, eventArgs e)
    {
        if (!this.checkConditions())
        {
            //Disable option
            isEnabled = false;
            title.color = Color.gray;
        }
        else
        {
            isEnabled = true;
            title.color = Color.black;
        }
    }

    public void OnPointerDown(PointerEventData pointer)
    {
        if (!isEnabled) return;
        BattleManager bm = BattleManager._instance;
        EventManager em = this.GetComponentInParent<EventManager>();
        if (isReward)
        {
            reward.addReward();
            em.unsubscribeDecisions();
            Destroy(this.transform.parent.gameObject);
            if (bm.player.HP > 0) bm.toggleScreen(screen.Path);
        }
        else
        {
            if(this.responses.Count == 0) 
            {
                em.unsubscribeDecisions();
                Destroy(this.transform.parent.gameObject);
                bm.toggleScreen(screen.Path);
            }
            else
            {
                int total = this.getResponseChances();
                int random = Random.Range(0, total);
                int index = 0;
                GameObject res = null;
                foreach(Response r in this.responses)
                {
                    if(random < (index + r.chance))
                    {
                        //found the option
                        res = r.e;
                        break;
                    }
                    else
                    {
                        index += r.chance;
                    }
                }
                em.unsubscribeDecisions();
                Destroy(this.transform.parent.gameObject);
                em.setEvent(res);
            }

        }
    }

    public int getResponseChances()
    {
        return this.responses.Sum(r => r.chance);
    }
}

[System.Serializable]
public struct Response
{
    [SerializeField]
    public int chance;
    public GameObject e;
}

[System.Serializable]
public struct startingDecision
{
    [SerializeField]
    public Reward reward;
    public string Text;
}