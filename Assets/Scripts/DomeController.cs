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
        //DEBUG
        if (Input.GetKeyDown("l"))
        {
            TakeDamage(50);
        }
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
