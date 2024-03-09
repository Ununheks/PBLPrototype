using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{

    public SpawnerManager _spawnerManager;

    private GameObject currentEnemy;
    private float lastShot = 0f;


    [SerializeField] public float reloadSpeed = 1f;
    [SerializeField] public float turretDamage = 10f;

    void Update()
    {
        if (_spawnerManager.enemies.Count > 0)
        {
            currentEnemy = _spawnerManager.enemies[0];
            transform.Find("TurretBarrelPivot").LookAt(currentEnemy.transform.position);
            if (Time.time > lastShot)
            {
                Shoot();
                lastShot = Time.time + reloadSpeed;
            }
        }
    }

    void Shoot()
    {
        currentEnemy.GetComponent<EnemyController>().TakeDamage(turretDamage);
    }
    
    public void SetTurretDamage(float damage)
    {
        turretDamage = damage;
    }
    
    public void SetTurretSpeed(float speed)
    {
        reloadSpeed = speed;
    }
}
