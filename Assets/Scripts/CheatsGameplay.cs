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
    Settings settings;

    public static int scoreAdded, energyAdded;

    private void Awake()
    {
        settings = FindObjectOfType<Settings>(); //Can't pass in a reference because it won't necessarily exist until the scene loads
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            scoreCounter.AddScore(1000);
            settings.SetCheatsEnabled();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            energyCounter.AddEnergy(100);
            settings.SetCheatsEnabled();
        }
    }

    public void AddScore()
    {
        scoreCounter.AddScore(1000);
        settings.SetCheatsEnabled();    
    }

    public void AddEnergy()
    {
        energyCounter.AddEnergy(100);
        settings.SetCheatsEnabled();       
    }

    public void SetRandomSeedToZero()
    {
        Random.InitState(0);
        settings.SetCheatsEnabled();
    }
}