using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{

    public SpawnerManager _spawnerManager;

    private GameObject currentEnemy;
    private float lastShot = 0f;

    private Transform turretTransform;
    private Quaternion originalRotation;

    List<GameObject> enemiesInRange = new List<GameObject>();


    [SerializeField] public float reloadSpeed = 1f;
    [SerializeField] public float turretDamage = 10f;

    public void Start()
    {
        turretTransform = transform.Find("TurretBarrelPivot");
        originalRotation = turretTransform.rotation;
    }

    void Update()
    {
        enemiesInRange.Clear();

        foreach (var enemy in _spawnerManager.enemies)
        {
            if (IsEnemyInRange(enemy))
            {
                enemiesInRange.Add(enemy);
            }
        }

        if (enemiesInRange.Count > 0)
        {
            currentEnemy = enemiesInRange[0];
            turretTransform.LookAt(currentEnemy.transform.position + Vector3.up * 0.3f);

            if (Time.time > lastShot)
            {
                Shoot();
                lastShot = Time.time + reloadSpeed;
            }
        }
        else
        {
            turretTransform.rotation = originalRotation;
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

    bool IsEnemyInRange(GameObject enemy)
    {
        Collider viewConeCollider = turretTransform.Find("ViewCone").GetComponent<Collider>();

        if (viewConeCollider.bounds.Intersects(enemy.GetComponent<Collider>().bounds))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
