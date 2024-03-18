using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretManager : MonoBehaviour
{

    public GameObject turretPrefab;
    public GameObject turretPreviewPrefab;

    [SerializeField] private Camera _playerCamera;
    [SerializeField] private SpawnerManager _spawnerManager;

    public List<GameObject> turrets;
    public List<GameObject> previewTurret;

    float _playerHeight = 1.4f;
    private bool _isHolding = false;

    void Update()
    {
        if(!_isHolding && Input.GetKeyDown("n"))
        {
            SpawnPreviewTurret();
            _isHolding = true;
        }
        if(_isHolding)
        {
            Vector3 spawnPosition = new Vector3(
            _playerCamera.transform.position.x + _playerCamera.transform.forward.x * 4f,
            _playerCamera.transform.position.y - _playerHeight,
            _playerCamera.transform.position.z + _playerCamera.transform.forward.z * 4f);
            previewTurret[0].transform.position = spawnPosition;
        }
        if(_isHolding && Input.GetKeyDown("m"))
        {
            SpawnTurret();
            _isHolding = false;
            Destroy(previewTurret[0]);
            previewTurret.Remove(previewTurret[0]);
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

    void SpawnPreviewTurret()
    {
        Vector3 spawnPosition = new Vector3(
            _playerCamera.transform.position.x + _playerCamera.transform.forward.x * 4f,
            _playerCamera.transform.position.y - _playerHeight,
            _playerCamera.transform.position.z + _playerCamera.transform.forward.z * 4f);

        GameObject turret = Instantiate(turretPreviewPrefab, spawnPosition, Quaternion.identity);
        previewTurret.Add(turret);
    }
}
