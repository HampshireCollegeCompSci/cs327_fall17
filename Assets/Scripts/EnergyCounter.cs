// Author(s): Paul Calande, Yixiang Xu, Wm. Josiah Erikson

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using SimpleJSON;

public class EnergyCounter : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The amount of energy the player currently has. The initial amount is Remote Settings. This is just for debug viewing.")]
    int energy = 10;
    [SerializeField]
    [Tooltip("The maximum amount of energy the player can have in the energy meter. Populated by Remote Settings.")]
    public int maxEnergyInMeter;
    [SerializeField]
    [Tooltip("Reference to the energy UI text.")]
    Text textEnergy;
    [SerializeField]
    [Tooltip("Reference to the GameFlow instance.")]
    GameFlow gameFlow;
    [SerializeField]
    [Tooltip("Reference to the Tuning JSON.")]
    TextAsset tuningJSON;
    [SerializeField]
    [Tooltip("The prefix for the Energy: string.")]
    string prefix;
    [SerializeField]
    [Tooltip("The Rising Text prefab to instantiate when energy is gained or lost.")]
    GameObject risingTextPrefab;
    [SerializeField]
    [Tooltip("Reference to the EnergyMeter instance.")]
    EnergyMeter energyMeter;
    [SerializeField]
    [Tooltip("Populated by Remote Settings")]
    public int initialEnergy;

    // The highest amount of energy achieved over the course of the game.
    int peakEnergy;

    void Tune()
    {
        //var json = JSON.Parse(tuningJSON.ToString());
        //energy = json["starting energy"].AsInt;
        energy = initialEnergy; //populated by Remote Settings
        //maxEnergyInMeter will be controlled by Remote Settings
        //maxEnergyInMeter = json["max energy in meter"].AsInt;
    }

    void Start()
    {
        Tune();

        UpdateEnergy();
    }

    public void AddEnergy(int amount)
    {
        energy += amount;
        /*
        if (energy > maxEnergy)
        {
            energy = maxEnergy;
        }
        */

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

    public void PopUp(string pref, int amount)
    {
        Vector3 popOutPos = new Vector3(transform.localPosition.x + 1.2f * GetComponent<RectTransform>().rect.width, transform.localPosition.y, transform.localPosition.z);
        
        GameObject risingTextObj = Object.Instantiate(risingTextPrefab, transform.parent.transform, false);
        risingTextObj.GetComponent<RisingText>().SetText(pref + (amount).ToString());
        risingTextObj.GetComponent<Text>().color = Color.green;
        risingTextObj.transform.localPosition = popOutPos;
    }

    private void UpdateEnergy()
    {
        textEnergy.text = prefix + energy.ToString();
        //Keep track of peak energy for Analytics
        if (energy > peakEnergy)
        { peakEnergy = energy; }
        //Update the energy meter
        energyMeter.UpdateEnergyMeter();
    }

    public int GetPeakEnergy()
    {
        return peakEnergy;
    }

    public int GetMaxEnergyInMeter()
    {
        return maxEnergyInMeter;
    }

    public int GetCurrentEnergy()
    {
        return energy;
    }
}