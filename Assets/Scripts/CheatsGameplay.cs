// Author(s): Paul Calande, Wm. Josiah Erikson

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheatsGameplay : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Reference to the ScoreCounter.")]
    ScoreCounter scoreCounter;
    [SerializeField]
    [Tooltip("Reference to the EnergyCounter.")]
    EnergyCounter energyCounter;
    [SerializeField]
    [Tooltip("Reference to the BlockSpawner.")]
    BlockSpawner blockSpawner;
    [SerializeField]
    [Tooltip("Reference to the child text.")]
    Text text;
    [SerializeField]
    [Tooltip("Reference to the Grid.")]
    Grid grid;

    Settings settings;

    bool isSpawningCheatingEnabled;

    bool isSpawningOneTileEnabled;

    private void Awake()
    {
        // Can't pass in a reference because it won't necessarily exist until the scene loads.
        settings = FindObjectOfType<Settings>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            AddScore(1000);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            AddScore(999);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            energyCounter.AddEnergy(100);
            settings.SetCheatsEnabled();
        }
    }

    public void AddScore()
    {
        AddScore(1000);
    }

    public void SetEnergyTo1()
    {
        int newEnergy = 1;
        energyCounter.SetEnergy(newEnergy);
        settings.SetCheatsEnabled();
    }

    public void AddEnergy()
    {
        energyCounter.AddEnergy(100);
        settings.SetCheatsEnabled();       
    }

    public void WipeSavedData()
    {
        //Wipes all saved data
        PlayerPrefs.DeleteAll();
        Settings.Instance.SetTutorialModeEnabled(true);
    }

    public void SetRandomSeedToZero()
    {
        Random.InitState(0);
        settings.SetCheatsEnabled();
    }

    public void SpawningSameBlock()
    {
        if (isSpawningCheatingEnabled)
        {
            blockSpawner.CheatSpawning(false);
            text.text = "Spawn Same Block";
            isSpawningCheatingEnabled = false;
        }
        else
        {
            blockSpawner.CheatSpawning(true);
            text.text = "Spawn Normal Block";
            settings.SetCheatsEnabled();
            isSpawningCheatingEnabled = true;
        }    
    }

    public void SpawnOneTileBlock()
    {
        if (isSpawningOneTileEnabled)
        {
            blockSpawner.OneTileCheatSpawning(false);
            text.text = "Spawn One Tile Block";
            isSpawningOneTileEnabled = false;
        }
        else
        {
            blockSpawner.OneTileCheatSpawning(true);
            text.text = "Spawn Normal Block";
            settings.SetCheatsEnabled();
            isSpawningOneTileEnabled = true;
        }
    }
    
    void AddScore(int amount)
    {
        scoreCounter.AddScore(amount);
        settings.SetCheatsEnabled();
        // Force event status to update.
        grid.ForceOnSquaresCleared();
    }
}