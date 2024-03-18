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

    public Quaternion newRotation;

    float _playerHeight = 1.4f;
    private bool _isHolding = false;
    [SerializeField] private bool _isLookingAtTurret = false;
    [SerializeField] private int _lastLookedAtTurret;

    void Update()
    {
        Quaternion cameraRotation = _playerCamera.transform.rotation;

        Vector3 euler = cameraRotation.eulerAngles;
        euler.x = 0f; // Set pitch to 0
        euler.z = 0f; // Set roll to 0

        newRotation = Quaternion.Euler(euler);

        if (_isLookingAtTurret && Input.GetKeyDown("n") && !_isHolding)
        {
            Destroy(turrets[_lastLookedAtTurret]);
            turrets.Remove(turrets[_lastLookedAtTurret]);

            SpawnPreviewTurret();
            _isHolding = true;
        }

        if(!_isHolding && Input.GetKeyDown("n") && !_isLookingAtTurret)
        {
            SpawnPreviewTurret();
            _isHolding = true;
        }
        if(_isHolding)
        {
            Vector3 spawnPosition = new Vector3(
            _playerCamera.transform.position.x + _playerCamera.transform.forward.x * 4f,
            _playerCamera.transform.position.y - _playerHeight + 0.1f,
            _playerCamera.transform.position.z + _playerCamera.transform.forward.z * 4f);
            previewTurret[0].transform.position = spawnPosition;

            Transform barrelTransform = previewTurret[0].transform.Find("TurretBarrelPivot");
            barrelTransform.rotation = newRotation;
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

        Transform barrelTransform = turret.transform.Find("TurretBarrelPivot");
        barrelTransform.rotation = newRotation;

        turrets.Add(turret);
    }

    void SpawnPreviewTurret()
    {
        Vector3 spawnPosition = new Vector3(
            _playerCamera.transform.position.x + _playerCamera.transform.forward.x * 4f,
            _playerCamera.transform.position.y - _playerHeight + 0.1f,
            _playerCamera.transform.position.z + _playerCamera.transform.forward.z * 4f);

        GameObject turret = Instantiate(turretPreviewPrefab, spawnPosition, Quaternion.identity);

        Transform barrelTransform = turret.transform.Find("TurretBarrelPivot");
        barrelTransform.rotation = newRotation;

        previewTurret.Add(turret);
    }

    public void SetIsLookingAtTurret(bool x)
    {
        _isLookingAtTurret = x;
    }

    public void SetLastLookedAtTurret(int x)
    {
        _lastLookedAtTurret = x;
    }

    public int GetLastLookedAtTurret()
    {
        return _lastLookedAtTurret;
    }
    public bool GetIsHolding()
    {
        return _isHolding;
    }
}
