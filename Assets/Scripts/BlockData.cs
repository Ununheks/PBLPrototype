using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlockData
{
    public BlockType BlockType { get; set; }
    public int PointValue { get; set; }
    public int ColumnID { get; set; }
    public int YID { get; set; }
    public float HP { get; set; }
    public float StartHP { get; set; }
    public SandManager SandManager { get; set; }
    public bool Visible { get; set; }

    public BlockData(BlockType blockType, int pointValue, int columnID, int yID, float startHP, SandManager sandManager)
    {
        BlockType = blockType;
        PointValue = pointValue;
        ColumnID = columnID;
        YID = yID;
        HP = startHP;
        StartHP = startHP;
        SandManager = sandManager;
    }
}

