using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnerManager : MonoBehaviour
{
    private int spawnerRange = 50;
    private int innerSquareRange = 20;

    public GameObject enemyPrefab;

    public List<GameObject> enemies;

    [SerializeField] private GameManager _gameManager;
    private void Update()
    {
        //DEBUG
        if (Input.GetKeyDown("p"))
        {
            SpawnWave();
        }
    }

    public void SpawnWave()
    {
        int numberOfEnemies = Random.Range(3, 6);
        for (int i = 0; i < numberOfEnemies; i++)
        {
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        Vector2 random = RandomPosition(spawnerRange, innerSquareRange);
        GameObject enemy = Instantiate(enemyPrefab, new Vector3(random.x, 13.5f, random.y), Quaternion.identity);
        enemy.transform.parent = transform;
        enemy.GetComponent<EnemyController>()._gameManager = _gameManager;
        enemies.Add(enemy);
    }

    Vector2 RandomPosition(int spawnerRange, int innerSquare)
    {
        while (true)
        {
            int x = Random.Range(-spawnerRange / 2, spawnerRange / 2);
            int y = Random.Range(-spawnerRange / 2, spawnerRange / 2);

            if (x < -innerSquare / 2 || x >= innerSquare / 2 || y < -innerSquare / 2 || y >= innerSquare / 2)
            {
                return new Vector2(x, y);
            }
        }
    }
}
