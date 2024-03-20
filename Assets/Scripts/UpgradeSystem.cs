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
                button1.onClick.Invoke();
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                button2.onClick.Invoke();
            }
            else if (Input.GetKeyDown(KeyCode.C))
            {
                button3.onClick.Invoke();
            }
            else if (Input.GetKeyDown(KeyCode.B))
            {
                button4.onClick.Invoke();
            }
        }
        else
        {
            SetButtonVisibility(false);
        }
    }

    void UpgradeDamage()
    {
        if (IronCounter.ironOreDestroyedCount >= 1)
        {
            Debug.Log("Upgraded damage");
            foreach (GameObject turret in _TurretManager.turrets)
            {
                turret.GetComponent<TurretController>().SetTurretDamage(turret.GetComponent<TurretController>().turretDamage + 20); 
            }
            IronCounter.ironOreDestroyedCount--;
            _gameManager.UpdateIronText(IronCounter.ironOreDestroyedCount);
        }
        else
        {
            Debug.Log("Insufficient funds");
        }
    }

    void UpgradeSpeed()
    {
        if (IronCounter.ironOreDestroyedCount >= 1)
        {
            Debug.Log("Upgraded Speed");
            foreach (GameObject turret in _TurretManager.turrets)
            {
                if (turret.GetComponent<TurretController>().reloadSpeed >= 0.2)
                    turret.GetComponent<TurretController>().SetTurretSpeed(turret.GetComponent<TurretController>().reloadSpeed - 0.05f); 
            }
            IronCounter.ironOreDestroyedCount--;
            _gameManager.UpdateIronText(IronCounter.ironOreDestroyedCount);
        }
        else
        {
            Debug.Log("Insufficient funds");
        }
    }

    void HealDome()
    {
        if (_gameManager.GetScore() >= 20)
        {
            _gameManager.AddPoints(-20);
            Debug.Log("Healed");
            _domeController.TakeDamage(-60);
        }
        else
        {
            Debug.Log("Insufficient funds");
        }
    }

    void IncreaseOwnedTurrets()
    {
        if(_TurretManager.GetTurretsHad() < 6)
        {
            if (_gameManager.GetScore() >= 10)
            {
                _gameManager.AddPoints(-10);
                _TurretManager.AddTurret();
                Debug.Log(_TurretManager.GetTurretsHad());
            }
        }
        
    }


    void SetButtonVisibility(bool isVisible)
    {
        button1.gameObject.SetActive(isVisible);
        button2.gameObject.SetActive(isVisible);
        button3.gameObject.SetActive(isVisible);
        button4.gameObject.SetActive(isVisible);
    }
}
