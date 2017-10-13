//Author: Wm. Josiah Erikson
//This class is just for a stand-in energy meter. It probably will get replaced with something else soon.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyMeter : MonoBehaviour
{
    private RectTransform rT; //For storing the rectTransform component
    float xPosition;
    [SerializeField]
    [Tooltip("yPosition. For debug viewing purposes only.")]
    float yPosition;
    [SerializeField]
    [Tooltip("Current energy. For debug viewing purposes only.")]
    int currentEnergy;
    [SerializeField]
    float percentOfMax;
    [SerializeField]
    [Tooltip("Maximum energy in counter. For debug viewing purposes only.")]
    float maxEnergy; //Maximum energy in meter
    [SerializeField]
    [Tooltip("Reference to the EnergyCounter.")]
    EnergyCounter ourEnergyCounter;
    [SerializeField]
    [Tooltip("Size Delta. For debug viewing purposes only.")]
    float sizeDelta;//For keeping track of our y sizeDelta


    // Use this for initialization
    void Start()
    {
        rT = GetComponent<RectTransform>(); //Get the rect transform component of this game object. For efficiency, let's just do it this one time
        xPosition = transform.localPosition.x; //Our x position shouldn't change
    }

    public void UpdateEnergyMeter()
    {
        maxEnergy = ourEnergyCounter.GetMaxEnergyInMeter(); //Get max energy
        currentEnergy = ourEnergyCounter.GetCurrentEnergy(); //Get current energy
        percentOfMax = currentEnergy / maxEnergy * 100;
        //I don't know exactly why 230 is the magic number. It's close to the size of this object but not exactly. I would like to fix this
        //as it's probably easily breakable!!
        sizeDelta = -230.0f + (percentOfMax * 2.3f); //sizeDelta should be between -230 (invisible) and 0 (full size)
        yPosition = sizeDelta / 2; //I have to move the y position by half as much to keep it in the same place
        rT.sizeDelta = new Vector2(rT.sizeDelta.x, sizeDelta);
        transform.localPosition = new Vector2(xPosition, yPosition);

    }
}
