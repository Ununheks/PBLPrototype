using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IronCounter : MonoBehaviour
{
    public static event Action<int> OnIronOreDestroyed;
    public static int ironOreDestroyedCount = 0;
    private void OnDestroy()
    {
        //ironOreDestroyedCount++;
        OnIronOreDestroyed?.Invoke(ironOreDestroyedCount);
        //Debug.Log("IronOre destroyed. Count: " + ironOreDestroyedCount);
    }
}
