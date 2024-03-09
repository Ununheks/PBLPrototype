using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DomeController : MonoBehaviour
{
    private int hp = 100;
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }
    
    public void TakeDamage(int damage)
    {
        hp -= damage;
    }

    public int getHealth()
    {
        return hp;
    }
}
