// Author(s): Paul Calande

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatsGameplay : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Reference to the ScoreCounter.")]
    ScoreCounter scoreCounter;
    [SerializeField]
    [Tooltip("Reference to the EnergyCounter.")]
    EnergyCounter energyCounter;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            scoreCounter.AddScore(1000);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            energyCounter.AddEnergy(100);
        }
    }
}