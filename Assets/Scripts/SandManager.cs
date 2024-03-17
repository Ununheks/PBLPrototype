using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SandManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> _blockPrefabs;
    [SerializeField] private List<float> _blockFrequencies;
    [SerializeField] private int _gridWidth = 10;
    [SerializeField] private int _gridHeight = 5;
    [SerializeField] private int _gridDepth = 10;

    private List<List<BlockController>> _blockColumns;
    private List<List<BlockData>> _blockDataColumns;

    void Start()
    {
        _blockColumns = new List<List<BlockController>>();
        _blockDataColumns = new List<List<BlockData>>();

        GenerateBlocks();

        SetVisibility();

        SpawnBlocks();
    }

    private void GenerateBlocks()
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
                List<BlockData> column = new List<BlockData>();
                _blockDataColumns.Add(column);
                columnID++;
                for (int y = 0; y < _gridHeight; y++)
                {
                    // Choose a random block prefab index based on the provided frequencies
                    int prefabIndex = ChooseRandomPrefabIndex();

                    // Create an instance of the appropriate subclass based on the prefabIndex
                    BlockData blockData = null;

                    int SphereRange = 10;
                    
                    if (y == _gridHeight - 3 && !((x == offsetX || x == offsetX + 1 || x == offsetX - 1) && (z == offsetZ || z == offsetZ + 1 || z == offsetZ - 1)))
                    {
                        blockData = new BlockData(BlockType.BEDROCK, 1, columnID, y, 2000, this);
                    }
                    else if ((x == offsetX || x == offsetX + 1 || x == offsetX - 1) && (y == _gridHeight - 2 || y == _gridHeight - 1) && (z == offsetZ || z == offsetZ + 1 || z == offsetZ - 1))
                    {
                        blockData = new BlockData(BlockType.AIR, 1, columnID, y, 2000, this);
                    }
                    else if (((x >= offsetX - SphereRange) && (x <= offsetX + SphereRange)) && (y == _gridHeight - 1 || y == _gridHeight - 2) && ((z >= offsetZ - SphereRange) && (z <= offsetZ + SphereRange)))
                    {
                        blockData = new BlockData(BlockType.BEDROCK, 1, columnID, y, 2000, this);
                    }
                    else
                    {
                        switch (prefabIndex)
                        {
                            case 0: // SandController
                                blockData = new BlockData(BlockType.SAND, 1, columnID, y, 1, this);
                                break;
                            case 1: // StoneController
                                blockData = new BlockData(BlockType.ROCK, 1, columnID, y, 1, this);
                                break;
                            case 2:
                                blockData = new BlockData(BlockType.BEDROCK, 1, columnID, y, 2000, this);
                                break;
                            case 3:
                                blockData = new BlockData(BlockType.AIR, 1, columnID, y, 2000, this);
                                break;
                            // Add more cases for additional types if needed
                            default:
                                Debug.LogError("Invalid prefabIndex!");
                                return;
                        }
                    }

                    // Add the instantiated block controller to the list
                    column.Add(blockData);
                }
            }
        }
    }

    private void SetVisibility()
    {
        foreach (List<BlockData> column in _blockDataColumns)
        {
            foreach (BlockData blockData in column)
            {
                if (blockData.YID == _gridHeight - 1)
                {
                    blockData.Visible = true;
                }
                else
                {
                    UpdateBlockVisibility(blockData);
                }
            }
        }
        
        foreach (List<BlockData> column in _blockDataColumns)
        {
            foreach (BlockData blockData in column)
            {
                if (blockData.BlockType == BlockType.AIR)
                {
                    //UpdateAdjacentVisibility(blockData);
                    
                    foreach (BlockData adjacentBlock in GetAdjacentBlockData(blockData))
                    {
                        adjacentBlock.Visible = true;
                    }
                }
            }
        }
    }

    public void UpdateAdjacentVisibility(BlockData blockData)
    {
        foreach (BlockData adjacentBlock in GetAdjacentBlockData(blockData))
        {
            UpdateBlockVisibility(adjacentBlock);
            if (adjacentBlock.Visible)
            {
                SpawnBlock(adjacentBlock);
            }
        }
    }

    private void SpawnBlock(BlockData blockData)
    {
        // Don't spawn blocks that are already destroyed
        if (blockData.BlockType == BlockType.EMPTY)
        {
            Debug.Log("Block is empty, skipping spawn.");
            return;
        }

        GameObject prefabToInstantiate = null;

        switch (blockData.BlockType)
        {
            case BlockType.SAND:
                prefabToInstantiate = _blockPrefabs[0];
                break;
            case BlockType.ROCK:
                prefabToInstantiate = _blockPrefabs[1];
                break;
            case BlockType.BEDROCK:
                prefabToInstantiate = _blockPrefabs[2];
                break;
            case BlockType.AIR:
                prefabToInstantiate = _blockPrefabs[3];
                break;
            // Add more cases for additional types if needed
            default:
                Debug.LogError("Invalid BlockType!");
                return; // Skip instantiation if BlockType is invalid
        }

        if (prefabToInstantiate != null)
        {
            // Instantiate the prefab at the appropriate position
            GameObject instantiatedPrefab = Instantiate(prefabToInstantiate, new Vector3((blockData.ColumnID / _gridWidth) - (_gridWidth / 2f), blockData.YID + 0.5f, (blockData.ColumnID % _gridDepth) - (_gridDepth / 2f)), Quaternion.identity);

            // Set the SandManager as the parent of the instantiated prefab
            instantiatedPrefab.transform.parent = transform;

            Renderer renderer = instantiatedPrefab.GetComponent<Renderer>();

            if (renderer != null)
            {
                Color colorVariation = new Color(Random.Range(-0.02f, 0.02f), Random.Range(-0.02f, 0.02f), Random.Range(-0.02f, 0.02f));

                renderer.material.color += colorVariation;
            }
                    

            // Attach BlockData to the instantiated prefab
            BlockController blockController = instantiatedPrefab.GetComponent<BlockController>();
            if (blockController != null)
            {
                blockController.BlockData = blockData;
                // Add the BlockController to the appropriate list
                _blockColumns[blockData.ColumnID].Add(blockController);
            }
            else
            {
                Debug.LogError("BlockController component not found on the instantiated prefab: " + prefabToInstantiate.name);
            }
        }
    }


    private List<BlockData> GetAdjacentBlockData(BlockData blockData)
    {
        int x = blockData.ColumnID / _gridWidth;
        int y = blockData.YID;
        int z = blockData.ColumnID % _gridDepth;

        List<BlockData> adjacentBlocks = new List<BlockData>();

        if (IsValidBlockPosition(x + 1, y, z) && !_blockDataColumns[(x + 1) * _gridWidth + z][y].Visible)
            adjacentBlocks.Add(_blockDataColumns[(x + 1) * _gridWidth + z][y]);
        if (IsValidBlockPosition(x - 1, y, z) && !_blockDataColumns[(x - 1) * _gridWidth + z][y].Visible)
            adjacentBlocks.Add(_blockDataColumns[(x - 1) * _gridWidth + z][y]);
        if (IsValidBlockPosition(x, y + 1, z) && !_blockDataColumns[x * _gridWidth + z][y + 1].Visible)
            adjacentBlocks.Add(_blockDataColumns[x * _gridWidth + z][y + 1]);
        if (IsValidBlockPosition(x, y - 1, z) && !_blockDataColumns[x * _gridWidth + z][y - 1].Visible)
            adjacentBlocks.Add(_blockDataColumns[x * _gridWidth + z][y - 1]);
        if (IsValidBlockPosition(x, y, z + 1) && !_blockDataColumns[x * _gridWidth + z + 1][y].Visible)
            adjacentBlocks.Add(_blockDataColumns[x * _gridWidth + z + 1][y]);
        if (IsValidBlockPosition(x, y, z - 1) && !_blockDataColumns[x * _gridWidth + z - 1][y].Visible)
            adjacentBlocks.Add(_blockDataColumns[x * _gridWidth + z - 1][y]);

        return adjacentBlocks;
    }

    private bool IsValidBlockPosition(int x, int y, int z)
    {
        return x >= 0 && x < _gridWidth && y >= 0 && y < _gridHeight && z >= 0 && z < _gridDepth;
    }

    private void UpdateBlockVisibility(BlockData blockData)
    {
        bool leftBlock = false;
        bool rightBlock = false;
        bool topBlock = false;
        bool bottomBlock = false;
        bool frontBlock = false;
        bool backBlock = false;

        int x = blockData.ColumnID / _gridWidth;
        int y = blockData.YID;
        int z = blockData.ColumnID % _gridDepth;

        if (x + 1 < _gridWidth)
            rightBlock = _blockDataColumns[(x + 1) * _gridWidth + z][y] != null && _blockDataColumns[(x + 1) * _gridWidth + z][y].BlockType != BlockType.EMPTY;
        else
            rightBlock = true;

        if (x - 1 >= 0)
            leftBlock = _blockDataColumns[(x - 1) * _gridWidth + z][y] != null && _blockDataColumns[(x - 1) * _gridWidth + z][y].BlockType != BlockType.EMPTY;
        else
            leftBlock = true;

        if (y + 1 < _gridHeight)
            topBlock = _blockDataColumns[x * _gridWidth + z][y + 1] != null && _blockDataColumns[x * _gridWidth + z][y + 1].BlockType != BlockType.EMPTY;
        else
            topBlock = true;

        if (y - 1 >= 0)
            bottomBlock = _blockDataColumns[x * _gridWidth + z][y - 1] != null && _blockDataColumns[x * _gridWidth + z][y - 1].BlockType != BlockType.EMPTY;
        else
            bottomBlock = true;

        if (z + 1 < _gridDepth)
            frontBlock = _blockDataColumns[x * _gridWidth + z + 1][y] != null && _blockDataColumns[x * _gridWidth + z + 1][y].BlockType != BlockType.EMPTY;
        else
            frontBlock = true;

        if (z - 1 >= 0)
            backBlock = _blockDataColumns[x * _gridWidth + z - 1][y] != null && _blockDataColumns[x * _gridWidth + z - 1][y].BlockType != BlockType.EMPTY;
        else
            backBlock = true;

        // If any of the adjacent blocks are absent or not empty, set visibility to true
        if (!leftBlock || !rightBlock || !topBlock || !bottomBlock || !frontBlock || !backBlock)
        {
            blockData.Visible = true;
        }
        else
        {
            blockData.Visible = false;
        }
    }



    public void SpawnBlocks()
    {
        // Clear the blockList before spawning blocks
        _blockColumns.Clear();

        foreach (List<BlockData> column in _blockDataColumns)
        {
            List<BlockController> columnBlocks = new List<BlockController>();

            foreach (BlockData blockData in column)
            {
                // Check if the block is visible
                if (!blockData.Visible)
                    continue;

                GameObject prefabToInstantiate = null;

                switch (blockData.BlockType)
                {
                    case BlockType.SAND:
                        prefabToInstantiate = _blockPrefabs[0];
                        break;
                    case BlockType.ROCK:
                        prefabToInstantiate = _blockPrefabs[1];
                        break;
                    case BlockType.BEDROCK:
                        prefabToInstantiate = _blockPrefabs[2];
                        break;
                    case BlockType.AIR:
                        prefabToInstantiate = _blockPrefabs[3];
                        break;
                    // Add more cases for additional types if needed
                    default:
                        Debug.LogError("Invalid BlockType!");
                        continue; // Skip to the next block if BlockType is invalid
                }

                if (prefabToInstantiate != null)
                {
                    // Instantiate the prefab at the appropriate position
                    GameObject instantiatedPrefab = Instantiate(prefabToInstantiate, new Vector3((blockData.ColumnID / _gridWidth) - (_gridWidth / 2f), blockData.YID + 0.5f, (blockData.ColumnID % _gridDepth) - (_gridDepth / 2f)), Quaternion.identity);

                    // Set the SandManager as the parent of the instantiated prefab
                    instantiatedPrefab.transform.parent = transform;

                    Renderer renderer = instantiatedPrefab.GetComponent<Renderer>();

                    if (renderer != null)
                    {
                        Color colorVariation = new Color(Random.Range(-0.02f, 0.02f), Random.Range(-0.02f, 0.02f), Random.Range(-0.02f, 0.02f));

                        renderer.material.color += colorVariation;
                    }

                    // Attach BlockData to the instantiated prefab
                    BlockController blockController = instantiatedPrefab.GetComponent<BlockController>();
                    if (blockController != null)
                    {
                        blockController.BlockData = blockData;
                        columnBlocks.Add(blockController); // Add the BlockController to the columnBlocks list
                    }
                    else
                    {
                        Debug.LogError("BlockController component not found on the instantiated prefab: " + prefabToInstantiate.name);
                    }
                }
            }

            // Add the columnBlocks list to the blockList
            _blockColumns.Add(columnBlocks);
        }
    }


    private int ChooseRandomPrefabIndex()
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


    public void UpdateColumn(BlockData blockData)
    {
        BlockController foundBlock = FindNextBlock(blockData.ColumnID, blockData.YID);

        if (foundBlock is SandController)
        {
            SandController foundSand = foundBlock as SandController;
            foundSand.SetState(SandState.WAIT);
        }
    }

    BlockController FindNextBlock(int columnID, int yID)
    {
        List<BlockController> column = _blockColumns[columnID];
        BlockController foundBlock = null;
        foreach (BlockController blockController in column)
        {
            // Check if the block's YID is greater than the provided yID
            // and if the block's BlockType is not EMPTY or STONE
            if (blockController.BlockData.YID > yID &&
                blockController.BlockData.BlockType != BlockType.EMPTY &&
                blockController.BlockData.BlockType != BlockType.ROCK)
            {
                // If foundBlock is null or the current block's YID is less than the foundBlock's YID,
                // update foundBlock to the current block
                if (foundBlock == null || blockController.BlockData.YID < foundBlock.BlockData.YID)
                {
                    foundBlock = blockController;
                }
            }
        }

        return foundBlock;
    }

}
