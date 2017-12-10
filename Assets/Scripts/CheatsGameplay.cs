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
    [SerializeField]
    [Tooltip("Reference to the ScoreBlocks instance.")]
    ScoreBlocks scoreBlocks;

    Settings settings;

    bool isSpawningCheatingEnabled;

    bool isSpawningOneTileEnabled;

    private void Start()
    {
        // Can't pass in a reference because it won't necessarily exist until the scene loads.
        settings = FindObjectOfType<Settings>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            AddScoreFromSquares(4);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            AddScoreFromSquares(1);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            AddScoreFromSquares(11);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            AddEnergy(100);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            AddEnergy(1);
        }
    }

    public void AddScore()
    {
        AddScoreFromSquares(4);
    }

    void AddScore(int amount)
    {
        scoreCounter.AddScore(amount);
        settings.SetCheatsEnabled();
        // Force event status to update.
        grid.ForceOnSquaresCleared();
    }

    void AddScoreFromSquares(int amount)
    {
        AddScore(grid.GetScorePerSquare() * amount);
        for (int i = 0; i < amount; ++i)
        {
            scoreBlocks.ScoreBlockAdded();
        }

        /*
        float timeBetweenSquares = 0.05f;
        for (int i = 0; i < amount; ++i)
        {
            StartCoroutine(AddScoreFromSquareBitByBit(timeBetweenSquares * i));
        }
        */
    }

    IEnumerator AddScoreFromSquareBitByBit(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        AddScore(grid.GetScorePerSquare());
        scoreBlocks.ScoreBlockAdded();
    }

    public void SetEnergyTo1()
    {
        int newEnergy = 1;
        energyCounter.SetEnergy(newEnergy);
        settings.SetCheatsEnabled();
    }

    public void AddEnergy()
    {
        AddEnergy(100);
    }

    public void AddEnergy(int amount)
    {
        energyCounter.AddEnergy(amount);
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
}