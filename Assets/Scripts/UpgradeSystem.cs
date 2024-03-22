using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeSystem : MonoBehaviour
{
    [SerializeField] private DomeController _domeController;
    public GameManager _gameManager;
    public GameObject player;
    public GameObject buttonObject;
    public TurretManager _TurretManager;
    public Button button1;
    public Button button2;
    public Button button3;
    public Button button4;
    public TextMeshProUGUI _ironText;
    public float interactionDistance = 3f;

  


    private bool isInRange = false;

    void Start()
    {
        SetButtonVisibility(false);
        button1.onClick.AddListener(UpgradeDamage);
        button2.onClick.AddListener(UpgradeSpeed);
        button3.onClick.AddListener(HealDome);
        button4.onClick.AddListener(IncreaseOwnedTurrets);
    }
    
    void Update()
    {
        isInRange = Vector3.Distance(player.transform.position, buttonObject.transform.position) <= interactionDistance;

        if (isInRange)
        {
            SetButtonVisibility(true);
            if (Input.GetKeyDown(KeyCode.Z))
            {
                _gameManager.AttemptToPurchasePowerUpWithEffects(5, 0);
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                _gameManager.AttemptToPurchasePowerUpWithEffects(6, 0);
            }
            else if (Input.GetKeyDown(KeyCode.C))
            {
                _gameManager.AttemptToPurchasePowerUpWithEffects(7, 0);
            }
            else if (Input.GetKeyDown(KeyCode.B))
            {
                if (_TurretManager.GetTurretsHad() < 10)
                {
                    _gameManager.AttemptToPurchasePowerUpWithEffects(8, 0);
                }
            }
        }
        else
        {
            SetButtonVisibility(false);
        }
    }

    public void UpgradeDamage()
    {

            Debug.Log("Upgraded damage");
            foreach (GameObject turret in _TurretManager.turrets)
            {
                turret.GetComponent<TurretController>().SetTurretDamage(turret.GetComponent<TurretController>().turretDamage + 20); 
            }
            _gameManager.UpdateIronText(IronCounter.ironOreDestroyedCount);
    }

    public void UpgradeSpeed()
    {

            Debug.Log("Upgraded Speed");
            foreach (GameObject turret in _TurretManager.turrets)
            {
                if (turret.GetComponent<TurretController>().reloadSpeed >= 0.2)
                    turret.GetComponent<TurretController>().SetTurretSpeed(turret.GetComponent<TurretController>().reloadSpeed - 0.05f); 
            }
            _gameManager.UpdateIronText(IronCounter.ironOreDestroyedCount);
    }

    public void HealDome()
    {
            Debug.Log("Healed");
            _domeController.TakeDamage(-60);
    }

    public void IncreaseOwnedTurrets()
    {
        if(_TurretManager.GetTurretsHad() < 10)
        {
                _TurretManager.AddTurret();
                Debug.Log(_TurretManager.GetTurretsHad());
            
        }
        
    }


    void SetButtonVisibility(bool isVisible)
    {
        button1.gameObject.SetActive(isVisible);
        button2.gameObject.SetActive(isVisible);
        button3.gameObject.SetActive(isVisible);
        button4.gameObject.SetActive(isVisible);
    }

    public bool GetIsInRange()
    {
        return isInRange;
    }
}
