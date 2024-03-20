using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnerManager : MonoBehaviour
{
    private int spawnerRange = 10;

    public GameObject enemyPrefab;
    
    public List<GameObject> enemies;

    [SerializeField] private GameManager _gameManager;

    [SerializeField] public List<GameObject> attentionSigns;
    [SerializeField] private List<Transform> _spawners;

    private void Start()
    {
        for (int i = 0; i < attentionSigns.Count; i++)
        {
            _spawners[i] = attentionSigns[i].GetComponentsInChildren<Transform>()[3];
        }
    }

    public void changeVisibilityOfSign(GameObject sign)
    {
        sign.GetComponentsInChildren<MeshRenderer>()[0].enabled = !sign.GetComponentsInChildren<MeshRenderer>()[0].enabled;
        sign.GetComponentsInChildren<MeshRenderer>()[1].enabled = !sign.GetComponentsInChildren<MeshRenderer>()[1].enabled;
    }

    private void Update()
    {
        //DEBUG
        if (Input.GetKeyDown("p"))
        {
            SpawnWave(1);
        }
    }

    public void SpawnWave(int numberOfWave)
    {
        for (int i = 0; i < numberOfWave; i++)
        {
            int numberOfEnemies = Random.Range(3, 6);
            for (int j = 0; j < numberOfEnemies; j++)
            {
                SpawnEnemy(_spawners[i]);
            }

        }
    }

    private void SpawnEnemy(Transform spawner)
    {
        Vector2 random = RandomPosition(spawner, spawnerRange);
        GameObject enemy = Instantiate(enemyPrefab, new Vector3(random.x, 13.5f, random.y), Quaternion.identity);
        enemy.transform.parent = transform;
        enemy.GetComponent<EnemyController>()._gameManager = _gameManager;
        enemies.Add(enemy);
    }

    Vector2 RandomPosition(Transform spawnerPos, int spawnerRange)
    {
        var position = spawnerPos.position;
        int x = Random.Range((int)position.x - spawnerRange / 2, (int)position.x + spawnerRange / 2);
        int y = Random.Range((int)position.z - spawnerRange / 2, (int)position.z + spawnerRange / 2);
        
        return new Vector2(x, y);
    }
}
