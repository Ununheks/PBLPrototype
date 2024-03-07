using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretManager : MonoBehaviour
{

    public GameObject turretPrefab;

    [SerializeField] private Camera _playerCamera;
    [SerializeField] private SpawnerManager _spawnerManager;

    public List<GameObject> turrets;

    float _playerHeight = 1.4f;

    void Update()
    {
        if (Input.GetKeyDown("m"))
        {
            for (int i = 0; i < 1; i++)
            {
                SpawnTurret();
            }
        }
    }

    void SpawnTurret()
    {
        Vector3 spawnPosition = new Vector3(
            _playerCamera.transform.position.x + _playerCamera.transform.forward.x * 4f,
            _playerCamera.transform.position.y - _playerHeight,
            _playerCamera.transform.position.z + _playerCamera.transform.forward.z * 4f);

        GameObject turret = Instantiate(turretPrefab, spawnPosition, Quaternion.identity);
        turret.GetComponent<TurretController>()._spawnerManager = _spawnerManager;
        turret.transform.parent = transform;    
        turrets.Add(turret);
    }
}
