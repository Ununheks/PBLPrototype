using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        // Check if the player presses keys 1 through 5
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            AttemptToPurchasePowerUp(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            AttemptToPurchasePowerUpWithEffects(1, 1); // Call AttemptToPurchasePowerUpWithEffects for key 2
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            AttemptToPurchasePowerUpWithEffects(2, 0.5f); // Call AttemptToPurchasePowerUpWithEffects for key 3
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            AttemptToPurchasePowerUpWithEffects(3, 0.5f); // Call AttemptToPurchasePowerUpWithEffects for key 4
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


    void AttemptToPurchasePowerUpWithEffects(int powerUpIndex, float value)
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

                // Call the appropriate function on the PlayerController based on the power-up index
                switch (powerUpIndex)
                {
                    case 1:
                        playerController.IncreaseReach(value); // Call IncreaseReach with value 1
                        break;
                    case 2:
                        playerController.IncreaseDamage(value); // Call IncreaseDamage with value 0.5
                        break;
                    case 3:
                        playerController.IncreaseRadius(value); // Call IncreaseRadius with value 0.25
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

    void AttemptToPurchasePowerUp(int powerUpIndex)
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

                // Call the appropriate function on the PlayerController based on the power-up index
                switch (powerUpIndex)
                {
                    case 0:
                        // Add 10 seconds to the time left
                        //_timeLeft += 10;
                        //UpdateTimeUI(_timeLeft);
                        break;
                    case 4:
                        playerController.AllowHold(); // Call AllowHold function in PlayerController
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
        _scoreUI.text = score.ToString(); // Update the score UI text
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
}
