using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDisplay : MonoBehaviour
{
    private Enemy enemy;
    private Card card;

    void Start()
    {
        this.enemy = this.GetComponent<Enemy>();
        this.card = this.GetComponent<Card>();
        this.updateDisplay();
    }

    public void updateDisplay()
    {
        this.card.HP.text = $"{this.enemy.HP}";
        this.card.Damage.text = $"{this.enemy.damage}";
        this.card.MinRoll.text = $"{this.enemy.minRoll}+";
    }

}
