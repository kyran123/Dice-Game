using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class Decision : MonoBehaviour, IPointerDownHandler
{
    /*
        What can a decision do
        modify player HP
        modify player Coins
        add/remove items
        add/remove die
    */

    [SerializeField]
    public Reward reward = new Reward();

    public TMP_Text title;

    void Start()
    {
        this.enableDecision(this, new eventArgs {});
        BattleManager._instance.OnHandIsFull += this.enableDecision;
    }

    public void enableDecision(object sender, eventArgs e)
    {
        if (!reward.validate(BattleManager._instance))
        {
            //Disable option
            title.color = Color.gray;
        }
        else
        {
            title.color = Color.white;
        }
    }

    public void OnPointerDown(PointerEventData pointer)
    {
        BattleManager bm = BattleManager._instance;
        if (!reward.validate(bm)) return;
        reward.addReward();
        bm.OnHandIsFull -= this.enableDecision;
        Destroy(this.transform.parent.gameObject);
        bm.toggleScreen(screen.Path);
    }
}

[System.Serializable]
public struct startingDecision {
    [SerializeField]
    public Reward reward;
    public string Text;
}