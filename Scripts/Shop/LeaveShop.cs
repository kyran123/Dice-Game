using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LeaveShop : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData pointer)
    {
        BattleManager._instance.toggleScreen(screen.Path);
    }
}
