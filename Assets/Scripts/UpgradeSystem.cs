using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeSystem : MonoBehaviour
{
    public GameManager _gameManager;
    public GameObject player;
    public GameObject buttonObject;
    public TurretManager _TurretManager;
    public Button button1;
    public Button button2;
    public float interactionDistance = 3f;

    private bool isInRange = false;

    void Start()
    {
        SetButtonVisibility(false);
        button1.onClick.AddListener(UpgradeDamage);
        button2.onClick.AddListener(UpgradeSpeed);
    }

    void Update()
    {
        isInRange = Vector3.Distance(player.transform.position, buttonObject.transform.position) <= interactionDistance;

        if (isInRange)
        {
            SetButtonVisibility(true);
            if (Input.GetKeyDown(KeyCode.Z))
            {
                button1.onClick.Invoke();
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                button2.onClick.Invoke();
            }
        }
        else
        {
            SetButtonVisibility(false);
        }
    }

    void UpgradeDamage()
    {
        if (_gameManager.GetScore() >= 20)
        {
            _gameManager.AddPoints(-20);
            Debug.Log("Upgraded damage");
            foreach (GameObject turret in _TurretManager.turrets)
            {
                turret.GetComponent<TurretController>().SetTurretDamage(turret.GetComponent<TurretController>().turretDamage + 20); 
            }
        }
    }

    void UpgradeSpeed()
    {
        if (_gameManager.GetScore() >= 20)
        {
            _gameManager.AddPoints(-20);
            Debug.Log("Upgraded Speed");
            foreach (GameObject turret in _TurretManager.turrets)
            {
                if (turret.GetComponent<TurretController>().reloadSpeed >= 0.2)
                turret.GetComponent<TurretController>().SetTurretSpeed(turret.GetComponent<TurretController>().reloadSpeed - 0.05f); 
            }
        }    
    }
    
    void SetButtonVisibility(bool isVisible)
    {
        button1.gameObject.SetActive(isVisible);
        button2.gameObject.SetActive(isVisible);
    }
}
