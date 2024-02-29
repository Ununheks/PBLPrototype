using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> _blockPrefabs;
    [SerializeField] private List<float> _blockFrequencies;
    [SerializeField] private int _gridWidth = 10;
    [SerializeField] private int _gridHeight = 5;
    [SerializeField] private int _gridDepth = 10;

    private List<List<BlockController>> _sandColumns;

    void Start()
    {
        _sandColumns = new List<List<BlockController>>();

        SpawnSandGrid();
    }

    void SpawnSandGrid()
    {
        // Check if the provided list of frequencies matches the count of block prefabs
        if (_blockFrequencies.Count != _blockPrefabs.Count)
        {
            Debug.LogError("The count of block frequencies does not match the count of block prefabs!");
            return;
        }

        // Calculate center offset for x and z
        float offsetX = _gridWidth / 2f;
        float offsetZ = _gridDepth / 2f;
        int columnID = -1;
        for (int x = 0; x < _gridWidth; x++)
        {
            for (int z = 0; z < _gridDepth; z++)
            {
                List<BlockController> column = new List<BlockController>();
                _sandColumns.Add(column);
                columnID++;
                for (int y = 0; y < _gridHeight; y++)
                {
                    // Adjust spawn position to center the grid around (0, 0)
                    Vector3 spawnPosition = new Vector3(x - offsetX, y + 0.5f, z - offsetZ);

                    // Choose a random block prefab index based on the provided frequencies
                    int prefabIndex = ChooseRandomPrefabIndex();

                    // Instantiate the selected block prefab
                    GameObject blockObject = Instantiate(_blockPrefabs[prefabIndex], spawnPosition, Quaternion.identity);
                    blockObject.transform.parent = transform;

                    // Get the renderer component of the block object
                    Renderer renderer = blockObject.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        // Generate a random color variation within a certain range
                        Color colorVariation = new Color(Random.Range(-0.05f, 0.05f), Random.Range(-0.05f, 0.05f), Random.Range(-0.05f, 0.05f));
                        renderer.material.color += colorVariation; // Apply the color variation to the material color
                    }

                    // Get the block controller component
                    BlockController blockController = blockObject.GetComponent<BlockController>();
                    column.Add(blockController);

                    // Initialize column ID, Y ID, and column unique ID for the spawned sand object
                    if (blockController != null)
                    {
                        blockController.Init(columnID, y, this);
                    }
                    else
                    {
                        Debug.LogWarning("BlockController component not found on the block object.");
                    }
                }
            }
        }
    }


    int ChooseRandomPrefabIndex()
    {
        // Calculate total frequency
        float totalFrequency = 0f;
        foreach (float frequency in _blockFrequencies)
        {
            totalFrequency += frequency;
        }

        // Generate a random value within the total frequency range
        float randomValue = Random.Range(0f, totalFrequency);

        // Find the corresponding index based on the random value
        float accumulatedFrequency = 0f;
        int selectedIndex = -1;
        for (int i = 0; i < _blockFrequencies.Count; i++)
        {
            accumulatedFrequency += _blockFrequencies[i];
            if (randomValue <= accumulatedFrequency)
            {
                selectedIndex = i;
                break;
            }
        }

        if (selectedIndex != -1)
        {
            return selectedIndex;
        }
        else
        {
            Debug.LogError("Failed to select a block prefab index!");
            return 0; // Return the first index as a fallback
        }
    }


    public void UpdateColumn(int columnID, int yID)
    {
        BlockController foundBlock = FindNextBlock(columnID, yID);

        if (foundBlock is SandController)
        {
            SandController foundSand = foundBlock as SandController;
            foundSand.SetState(SandState.WAIT);
        }
    }

    BlockController FindNextBlock(int columnID, int yID)
    {
        List<BlockController> column = _sandColumns[columnID];
        BlockController foundBlock = null;
        foreach (BlockController blockController in column)
        {
            if (blockController.YID > yID)
            {
                if (foundBlock == null || blockController.YID < foundBlock.YID)
                {
                    foundBlock = blockController;
                }
            }
        }

        if (foundBlock != null)
        {
            return foundBlock;
        }
        else
        {
            return null;
        }
    }
}
