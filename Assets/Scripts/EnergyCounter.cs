// Author(s): Paul Calande, Yixiang Xu

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;

public class EnergyCounter : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The amount of energy the player currently has.")]
    int energy = 10;
    [SerializeField]
    [Tooltip("The amount of energy gotten for a tile cleared.")]
    int energyPerCell;
    [SerializeField]
    [Tooltip("Reference to the energy UI text.")]
    Text textEnergy;
    [SerializeField]
    GameFlow gameFlow;
    [SerializeField]
    TextAsset tuningJSON;

    void Tune()
    {
        var json = JSON.Parse(tuningJSON.ToString());
        energy = json["starting energy"];
        energyPerCell = json["energy per cell cleared"];
    }

    void Start()
    {
        Tune();

        UpdateEnergy();
    }

    public void AddEnergy()
    {
        energy += energyPerCell;
        UpdateEnergy();
    }

    public void RemoveEnergy(int amount)
    {
        energy -= amount;

        if (energy <= 0)
        {
            energy = 0;
            gameFlow.GameOver(GameFlow.GameOverCause.NoMoreEnergy);
        }

        UpdateEnergy();
    }

    private void UpdateEnergy()
    {
        textEnergy.text = "Energy: " + energy.ToString();
    }
}