using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //[SerializeField] private float _gameTime;
    [SerializeField] private float _score;
    //[SerializeField] private float _timeLeft;
    [SerializeField] private float _totalScore; // Variable to hold the total score
    //[SerializeField] private TextMeshProUGUI _timeUI;
    [SerializeField] private TextMeshProUGUI _scoreUI; // Add reference to the score UI text
    [SerializeField] private TextMeshProUGUI _totalScoreUI; // Reference to the total score UI text
    [SerializeField] private TextMeshProUGUI _gameOver; 
    [SerializeField] private GameObject _gameOverScreen;
    [SerializeField] private List<int> _powerUpCosts;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PhaseManager _phaseManager;
    [SerializeField] private DomeController _domeController;
    [SerializeField] private UpgradeSystem _upgradeSystem;
    [SerializeField] private TurretManager _turretManager;

    [SerializeField] private TextMeshProUGUI _timePrice;
    [SerializeField] private TextMeshProUGUI _reachPrice;
    [SerializeField] private TextMeshProUGUI _powerPrice;
    [SerializeField] private TextMeshProUGUI _radiusPrice;
    [SerializeField] private TextMeshProUGUI _holdPrice;
    [SerializeField] private TextMeshProUGUI _turretPrice;
    [SerializeField] private TextMeshProUGUI _turretDamagePrice;
    [SerializeField] private TextMeshProUGUI _turretSpeedPrice;

    public Button button1;
    public Button button2;
    public Button button3;
    public Button button4;
    public Button button5;

    private bool isRestarting = false;
    public TextMeshProUGUI _ironText;


    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();

                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(GameManager).Name);
                    _instance = singletonObject.AddComponent<GameManager>();
                }
            }

            return _instance;
        }
    }

    private GameManager() { }

    void Start()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        //_timeLeft = _gameTime;
        //UpdateTimeUI(_gameTime);
        UpdateScoreUI(_score); // Update the score UI text when the game starts
        _gameOver.text = "";
        IronCounter.OnIronOreDestroyed += UpdateIronText;

        SetButtonVisibility(false);

    }

    void Update()
    {


        /*if (_timeLeft > 0)
        {
            _timeLeft -= Time.deltaTime;
            UpdateTimeUI(_timeLeft);
        }
        else
        {
            _timeLeft = 0; // Ensure time left doesn't go negative

            // Set timescale to 0 to stop the game
            Time.timeScale = 0;

            // Enable the game over screen
            if (_gameOverScreen != null)
            {
                _gameOverScreen.SetActive(true);
                // Update the total score UI text when the game over screen is enabled
                if (_totalScoreUI != null)
                {
                    _totalScoreUI.text = "Total Score: " + _totalScore.ToString();
                }

                // Set the time left UI to "00:00"
                _timeUI.text = "00:00";
            }
        }*/
        if(_upgradeSystem.GetIsInRange())
        {
            SetButtonVisibility(true);
        }
        else
        {
            SetButtonVisibility(false);
        }

        // Check if the player presses keys 1 through 5
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            AttemptToPurchasePowerUp(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            AttemptToPurchasePowerUpWithEffects(1, 0.5f); // Call AttemptToPurchasePowerUpWithEffects for key 2
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            AttemptToPurchasePowerUpWithEffects(2, 0.2f); // Call AttemptToPurchasePowerUpWithEffects for key 3
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            AttemptToPurchasePowerUpWithEffects(3, 0.2f); // Call AttemptToPurchasePowerUpWithEffects for key 4
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            AttemptToPurchasePowerUp(4);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            playerController.IncreaseReach(20);
            playerController.IncreaseDamage(0.5f);
            playerController.IncreaseRadius(3);
        }
    }


    public void AttemptToPurchasePowerUpWithEffects(int powerUpIndex, float value)
    {
        // Check if the powerUpIndex is valid
        if (_upgradeSystem.GetIsInRange())
        {
            if (powerUpIndex >= 0 && powerUpIndex < _powerUpCosts.Count)
            {
                int powerUpCost = _powerUpCosts[powerUpIndex];
                // Check if the player has enough score to purchase the power-up
                if (_score >= powerUpCost)
                {
                    // Deduct the power-up cost from the player's score
                    _score -= powerUpCost;

                    // Update the UI
                    UpdateScoreUI(_score);
                    if (powerUpIndex == 8)
                    {
                        _powerUpCosts[powerUpIndex] = _powerUpCosts[powerUpIndex] + 1;
                    }
                    else
                    {
                        _powerUpCosts[powerUpIndex] = _powerUpCosts[powerUpIndex] * 2;
                    }

                    // Call the appropriate function on the PlayerController based on the power-up index

                    switch (powerUpIndex)
                    {
                        case 1:

                            playerController.IncreaseReach(value); // Call IncreaseReach with value 1
                            _reachPrice.text = "[2] Reach " + _powerUpCosts[powerUpIndex];
                            break;
                        case 2:
                            playerController.IncreaseDamage(value); // Call IncreaseDamage with value 0.5
                            _powerPrice.text = "[3] Power " + _powerUpCosts[powerUpIndex];
                            break;
                        case 3:
                            playerController.IncreaseRadius(value); // Call IncreaseRadius with value 0.25
                            _radiusPrice.text = "[4] Radius " + _powerUpCosts[powerUpIndex];
                            break;
                        case 5:
                            _upgradeSystem.UpgradeDamage();
                            _turretDamagePrice.text = "Turret Damage+ [Z] " + _powerUpCosts[powerUpIndex];
                            break;
                        case 6:
                            _upgradeSystem.UpgradeSpeed();
                            _turretSpeedPrice.text = "Turret Speed+ [X] " + _powerUpCosts[powerUpIndex];
                            break;
                        case 7:
                            _upgradeSystem.HealDome();
                            break;
                        case 8:
                            if (_turretManager.GetTurretsHad() < 10)
                            {
                                _upgradeSystem.IncreaseOwnedTurrets();
                                _turretPrice.text = "Buy Turret [B] " + _powerUpCosts[powerUpIndex];
                                break;
                            }
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    Debug.Log("Insufficient score to purchase this power-up!");
                }
            }
            else
            {
                Debug.Log("Invalid power-up index!");
            }
        }
    }

    void AttemptToPurchasePowerUp(int powerUpIndex)
    {
        if (_upgradeSystem.GetIsInRange())
        {
            // Check if the powerUpIndex is valid
            if (powerUpIndex >= 0 && powerUpIndex < _powerUpCosts.Count)
            {
                int powerUpCost = _powerUpCosts[powerUpIndex];


                // Check if the player has enough score to purchase the power-up
                if (_score >= powerUpCost)
                {
                    // Deduct the power-up cost from the player's score
                    _score -= powerUpCost;

                    // Update the UI
                    UpdateScoreUI(_score);
                    _powerUpCosts[powerUpIndex] = _powerUpCosts[powerUpIndex] * 2;

                    // Call the appropriate function on the PlayerController based on the power-up index
                    switch (powerUpIndex)
                    {
                        case 0:
                            // Add 10 seconds to the time left
                            //_timeLeft += 10;
                            //UpdateTimeUI(_timeLeft);
                            _timePrice.text = "[1] Time " + _powerUpCosts[powerUpIndex];
                            break;
                        case 4:
                            playerController.AllowHold(); // Call AllowHold function in PlayerController
                            _holdPrice.text = "[5] Hold " + _powerUpCosts[powerUpIndex];
                            break;
                        default:
                            Debug.Log("Power-up not implemented.");
                            break;
                    }
                }
                else
                {
                    Debug.Log("Insufficient score to purchase this power-up!");
                }
            }
            else
            {
                Debug.Log("Invalid power-up index!");
            }
        }
    }


    private void UpdateTimeUI(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        //_timeUI.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void AddPoints(int value)
    {
        _score += value;
        _totalScore += value; // Update the total score
        UpdateScoreUI(_score); // Update the score UI text when the score changes
    }

    private void UpdateScoreUI(float score)
    {
        _scoreUI.text = "Iron Ore: " + score.ToString(); // Update the score UI text
    }

    public void BaseTakeDamage(float damage)
    {
        if (_domeController.getHealth() > 0)
        {
            _domeController.TakeDamage(20);
            print(_domeController.getHealth());
            print("Base attacked!");
        }
        else
        {
            gameOver();
        }
    }

    private void gameOver()
    {
        if (!isRestarting)
        {
            isRestarting = true;
            StartCoroutine(RestartCountdown());
        }
    }
    
    IEnumerator RestartCountdown()
    {
        for (int i = 5; i >= 0; i--)
        {
            _gameOver.text = "Game over! \nSurvived " + _phaseManager.waveNumber + " wave!\nRestarting in " + i + " seconds";
            yield return new WaitForSeconds(1f);
        }

        RestartGame();
        isRestarting = false; // Reset the flag after the restart is complete
    }
    
    void RestartGame()
    {
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public float GetScore()
    {
        return _score;
    }
    
    public void UpdateIronText(int count)
    {
        _ironText.text = "Iron Ore: " + count;
    }

    void SetButtonVisibility(bool isVisible)
    {
        button1.gameObject.SetActive(isVisible);
        button2.gameObject.SetActive(isVisible);
        button3.gameObject.SetActive(isVisible);
        button4.gameObject.SetActive(isVisible);
        button5.gameObject.SetActive(isVisible);
    }
}
