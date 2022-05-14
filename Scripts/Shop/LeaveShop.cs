using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LeaveShop : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData pointer)
    {
        this.GetComponentInParent<ShopManager>().deleteItems();
        BattleManager._instance.toggleScreen(screen.Path);
    }
}
