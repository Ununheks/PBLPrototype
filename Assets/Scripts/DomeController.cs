using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DomeController : MonoBehaviour
{
    private float hp = 100;
    public Image healthBar;
    
    public void TakeDamage(int damage)
    {
        hp -= damage;
        healthBar.fillAmount = hp / 100;
    }

    public float getHealth()
    {
        return hp;
    }
}
