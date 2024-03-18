using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DomeController : MonoBehaviour
{
    private float hp = 150;
    public Image healthBar;
    
    public void TakeDamage(int damage)
    {
        hp -= damage;
        hp = Math.Clamp(hp, 0, 150);
        healthBar.fillAmount = hp / 100;
    }

    public float getHealth()
    {
        return hp;
    }
}
