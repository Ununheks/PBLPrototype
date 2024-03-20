using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.AI;
using Unity.AI.Navigation;

public class PhaseManager : MonoBehaviour
{
    [SerializeField] private int actualPhaze;
    [SerializeField] public int waveNumber = 0;
    [SerializeField] private SandManager _sandManager;
    [SerializeField] private SpawnerManager _spawnerManager;
    [SerializeField] private GameObject _doors;
    [SerializeField] private TextMeshProUGUI _timeUI;
    [SerializeField] private TextMeshProUGUI _phaseUI;
    [SerializeField] private float _timeLeft;
    [SerializeField] private GameObject _player;
    [SerializeField] private int domeSize;
    [SerializeField] private GameObject _sphereCollider;

    [SerializeField] private NavMeshSurface _navMeshSurface;
    private void Start()
    {
        _doors = Instantiate(_doors, new Vector3(0, 12.4f, 0), Quaternion.identity);
        ChangePhase(1);
    }

    private void ChangePhase(int phase)
    {
        actualPhaze = phase;
        switch (phase)
        {
            case 1:
                //print("zmieniono na faze kopania");
                _doors.SetActive(false);
                _sphereCollider.SetActive(true);
                _timeLeft = 5;
                break;
            case 2:
                //print("zmieniono na faze ustawiania");
                _doors.SetActive(true);
                
                PrepareWaves();
                
                if (_player.transform.position.y <= 13)
                {
                    _player.GetComponent<PlayerMovement>().enabled = false;
                    _player.transform.position = new Vector3(0, 14, 0);
                    StartCoroutine(ReturnControll());
                }
                _sphereCollider.SetActive(false);
                _timeLeft = 5;
                break;
            case 3:
                //print("zmieniono na faze walki");
                _navMeshSurface.BuildNavMesh();
                
                if (Vector3.Distance(_player.transform.position, new Vector3(0, 14, 0)) > domeSize)
                {
                    _player.GetComponent<PlayerMovement>().enabled = false;
                    _player.transform.position = new Vector3(0, 14, 0);
                    StartCoroutine(ReturnControll());
                }
                _sphereCollider.SetActive(true);
                waveNumber += 1;
                StartCoroutine(SpawnWaves(3));
                _timeLeft = 5;
                break;
            default:
                break;
        }
        updatePhaseUI();
    }

    private void PrepareWaves()
    {
        if (waveNumber >= 4)
        {
            foreach (var VARIABLE in _spawnerManager.attentionSigns)
            {
                if (VARIABLE.GetComponentsInChildren<MeshRenderer>()[0].enabled == false)
                {
                    _spawnerManager.changeVisibilityOfSign(VARIABLE);
                }
            }
        }
        else
        {
            for (int i = 0; i < waveNumber + 1; i++)
            {
                print(_spawnerManager.attentionSigns[i]);
                
                if (_spawnerManager.attentionSigns[i].GetComponentsInChildren<MeshRenderer>()[0].enabled == false)
                {
                    _spawnerManager.changeVisibilityOfSign(_spawnerManager.attentionSigns[i]);
                }
            }
        }
    }

    IEnumerator SpawnWaves(int numberOfWaves)
    {
        
        foreach (var VARIABLE in _spawnerManager.attentionSigns)
        {
            if (VARIABLE.GetComponentsInChildren<MeshRenderer>()[0].enabled == true)
            {
                _spawnerManager.changeVisibilityOfSign(VARIABLE);
            }
        }
        
        for (int i = 0; i < numberOfWaves; i++)
        {
            _spawnerManager.SpawnWave(waveNumber);
            yield return new WaitForSeconds(3);
        }
    }
    
    IEnumerator ReturnControll()
    {
        yield return new WaitForSeconds(1);
        _player.GetComponent<PlayerMovement>().enabled = true;
    }

    private void Update()
    {
        if (_timeLeft > 0)
        {
            _timeLeft -= Time.deltaTime;
            UpdateTimeUI(_timeLeft);
        }
        else
        {
            if (actualPhaze != 3)
            {
                ChangePhase(actualPhaze + 1);
            }
            else
            {
                ChangePhase(1);
            }
        }
    }
    
    private void UpdateTimeUI(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        _timeUI.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void updatePhaseUI()
    {
        switch (actualPhaze)
        {
            case 1:
                _phaseUI.text = "Digging Phase";
                break;
            case 2:
                _phaseUI.text = "Setup Phase";
                break;
            case 3:
                _phaseUI.text = "Wave: " + waveNumber;
                break;
        }
        
    }
}
