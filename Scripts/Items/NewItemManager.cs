using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewItemManager : MonoBehaviour
{
    public ItemContainer ItemContainer;

    void Start() {
        BattleManager._instance.OnAddItem += this.addItem;
        BattleManager._instance.OnRewardAdded += this.deHighlight;
    }

    public void addItem(object sender, eventArgs e) {
        this.toggle(true);
        this.ItemContainer.addItem(e.itemObject);
    }

    public void deHighlight(object sender, eventArgs e) {
        this.toggle(false);
    }

    public void toggle(bool toggleValue) {
        foreach(Transform obj in this.transform) {
            obj.gameObject.SetActive(toggleValue);
        }
    }

    public void skip() {
        //Call Battlemanager to show the path cards
        this.ItemContainer.removeItem();
        BattleManager._instance.toggleScreen(screen.Path);
        this.toggle(false);
    }
}
