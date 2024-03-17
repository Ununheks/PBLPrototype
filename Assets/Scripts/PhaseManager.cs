using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseManager : MonoBehaviour
{
    [SerializeField] private int actualPhaze;

    private void ChangePhase(int phase)
    {
        actualPhaze = phase;
        
    } 
}
