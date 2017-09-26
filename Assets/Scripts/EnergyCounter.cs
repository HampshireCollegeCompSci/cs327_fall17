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

    public void AddEnergy(int amount)
    {

    }

    public void RemoveEnergy(int amount)
    {

    }

    private void UpdateEnergy()
    {

    }
}