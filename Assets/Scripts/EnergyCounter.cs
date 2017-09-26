// Author(s): Paul Calande

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyCounter : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Reference to the energy UI text.")]
    Text textEnergy;
    [SerializeField]
    [Tooltip("The amount of energy the player currently has.")]
    int energy = 10;
    [SerializeField]
    GameFlow gameFlow;

    void Start()
    {
        UpdateEnergy();
    }

    public void AddEnergy(int amount)
    {
        energy += amount;
        UpdateEnergy();
    }

    public void RemoveEnergy(int amount)
    {
        energy -= amount;
        UpdateEnergy();

        if(energy <= 0)
        {
            gameFlow.GameOver(GameFlow.GameOverCause.NoMoreEnergy);
        }
    }

    private void UpdateEnergy()
    {
        textEnergy.text = "Energy: " + energy.ToString();
    }
}