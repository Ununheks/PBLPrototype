using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneController : BlockController
{
    public override void Init(int columnID, int yID, SandManager sandManager)
    {
        _hp = 10f;
        _blockType = BlockType.ROCK;
        _pointValue = 20;
        base.Init(columnID, yID, sandManager);
    }

    public override void BeforeDestroy()
    {
        _sandManager.UpdateColumn(_columnID, _yID);
    }
}
