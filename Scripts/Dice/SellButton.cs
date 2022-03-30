using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SellButton : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData pointer)
    {
        this.GetComponentInParent<DiceContainer>().remove();
    }
}
