using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Path : MonoBehaviour, IPointerDownHandler
{
    public Sprite questionSprite;

    public bool isRandom = false;

    public pathType type;

    void Start() {
        if(isRandom) this.GetComponent<Image>().sprite = questionSprite;
    }

    public void OnPointerDown(PointerEventData pointer)
    {
        BattleManager._instance.level ++;
        switch(type)
        {
            case pathType.Monster:
                BattleManager._instance.toggleScreen(screen.Battle);
                BattleManager._instance.generateEnemy();
            break;
        }
    }

}

public enum pathType
{
    Elite,
    Monster,
    Event,
    Shop
}