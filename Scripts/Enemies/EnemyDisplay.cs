using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDisplay : MonoBehaviour
{
    public Enemy enemy;
    public Card card;

    void Start()
    {
        this.updateDisplay();
    }

    public void updateDisplay()
    {
        this.card.Name.text = $"{this.enemy.enemyName}";
        this.card.HP.text = $"{this.enemy.HP}";
        this.card.Damage.text = $"{this.enemy.getDamage()}";
        this.card.MinRoll.text = $"{this.enemy.getMinRoll()}+";
    }

}
