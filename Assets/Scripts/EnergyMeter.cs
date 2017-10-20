//Author: Wm. Josiah Erikson
//This class is just for a stand-in energy meter. It probably will get replaced with something else soon.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyMeter : MonoBehaviour
{
    private RectTransform rT; //For storing the rectTransform component
    [SerializeField]
    [Tooltip("Current energy. For debug viewing purposes only.")]
    public int currentEnergy;
    [SerializeField]
    float percentOfMax;
    [SerializeField]
    [Tooltip("Maximum energy in counter. For debug viewing purposes only.")]
    float maxEnergy; //Maximum energy in meter
    [SerializeField]
    [Tooltip("Reference to the EnergyCounter.")]
    EnergyCounter ourEnergyCounter;


    // Use this for initialization
    void Awake()
    {
        rT = GetComponent<RectTransform>(); //Get the rect transform component of this game object. For efficiency, let's just do it this one time
        maxEnergy = ourEnergyCounter.GetMaxEnergyInMeter(); //Get max energy
    }

    public void UpdateEnergyMeter()
    {
        currentEnergy = ourEnergyCounter.GetCurrentEnergy(); //Get current energy
        percentOfMax = currentEnergy / maxEnergy;
        if (percentOfMax > 1) //You can get more than maxEnergy, the meter should just max out there.
        {
            percentOfMax = 1;
        }
        rT.anchorMax = new Vector2(rT.anchorMax.x, percentOfMax);
    }
}
