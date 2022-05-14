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

    void Start()
    {
        if (isRandom) this.GetComponent<Image>().sprite = questionSprite;
    }

    public void OnPointerDown(PointerEventData pointer)
    {
        switch (type)
        {
            case pathType.Monster:
                BattleManager._instance.generateEnemy();
                BattleManager._instance.toggleScreen(screen.Battle);
                break;
            case pathType.Elite:
                BattleManager._instance.generateEnemy(true);
                BattleManager._instance.toggleScreen(screen.Battle);
                break;
            case pathType.Event:
                BattleManager._instance.toggleScreen(screen.Event);
                break;
            case pathType.Shop:
                BattleManager._instance.toggleScreen(screen.Shop);
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