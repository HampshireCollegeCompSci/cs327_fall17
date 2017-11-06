// Author(s): Paul Calande, Yixiang Xu, Wm. Josiah Erikson

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
//using UnityEditor;
//using UnityEditor.Animations;

public class EnergyCounter : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The amount of energy the player currently has. The initial amount is populated by JSON.")]
    int energy = 10;
    [SerializeField]
    [Tooltip("The maximum amount of energy the player can have in the energy meter. Populated by JSON.")]
    int maxEnergyInMeter = 60;
    [SerializeField]
    [Tooltip("The list of value to separate the levels of energy gained. Populated by JSON.")]
    JSONArray energyThreshold;
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
    [Tooltip("Reference to the Grid.")]
    Grid grid;
    [SerializeField]
    [Tooltip("Reference to energy gain animator.")]
    Animator energyGainController;
    

    // The highest amount of energy achieved over the course of the game.
    int peakEnergy;

    // The position for rising text to pop up.
    Vector3 popUpPos;

    // The reference to block transform.
    Transform blockTransform;

    void Tune()
    {
        var json = JSON.Parse(tuningJSON.ToString());
        energy = json["starting energy"].AsInt;
        maxEnergyInMeter = json["max energy in meter"].AsInt;
        energyThreshold = json["energy level animation"].AsArray;
    }

    void Start()
    {
        Tune();
        //SetEnergyLevel();
        UpdateEnergy();
    }

    public void UpdateEnergyLevel()
    {
        if (energy < energyThreshold[0])
        {
            energyGainController.SetInteger("energyLevel", 0);
        }
        else if (energy < energyThreshold[1])
        {
            energyGainController.SetInteger("energyLevel", 1);
        }
        else if (energy < energyThreshold[2])
        {
            energyGainController.SetInteger("energyLevel", 2);
        }
        else
        {
            energyGainController.SetInteger("energyLevel", 3);
        }
    }

    /*public void SetEnergyLevel()
    {
        AnimatorController energyController = (AnimatorController)energyGainController.runtimeAnimatorController;
        AnimatorStateMachine stateMachine = energyController.layers[0].stateMachine;
        var states = stateMachine.states;
        for (int i = 0; i < states.Length; i++)
        {
            AnimatorState state = states[i].state;
            if(state.name.Equals("Inactive"))
            {
                for (int j = 0; j < state.transitions.Length; j++)
                {
                    AnimatorStateTransition tran = state.transitions[j];
                    string stateName = tran.destinationState.name;
                    if (tran.conditions.Length == 1)
                    {
                        tran.AddCondition(AnimatorConditionMode.Less, 0, "energyGained");
                    }

                    for (int l = 0; l < tran.conditions.Length; l++)
                    {
                        if (tran.conditions[l].parameter.Equals("energyGained"))
                        {
                            switch (stateName) {
                                case "EnergyGain(Very Low)": tran.RemoveCondition(tran.conditions[l]);
                                    tran.AddCondition(AnimatorConditionMode.Less, energyLevel[0] + 1, "energyGained");
                                    break;
                                case "EnergyGain(Low)":
                                    tran.RemoveCondition(tran.conditions[l]);
                                    tran.AddCondition(AnimatorConditionMode.Less, energyLevel[1] + 1, "energyGained");
                                    break;
                                case "EnergyGain(Medium)":
                                    tran.RemoveCondition(tran.conditions[l]);
                                    tran.AddCondition(AnimatorConditionMode.Less, energyLevel[2] + 1, "energyGained");
                                    break;
                                case "EnergyGain(High)":
                                    tran.RemoveCondition(tran.conditions[l]);
                                    tran.AddCondition(AnimatorConditionMode.Less, energyLevel[3] + 1, "energyGained");
                                    break;
                            }
                        }
                    }
                }
            }
        }
    }*/

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

    public void SetPopUpPos(Vector3 pos)
    {
        popUpPos = pos;
    }

    public void SetBlockTransform(Transform transform)
    {
        blockTransform = transform;
    }

    public void PopUp(string pref, int amount)
    {
        GameObject risingTextObj = Object.Instantiate(risingTextPrefab, blockTransform.parent.transform, false);
        risingTextObj.GetComponent<RisingText>().SetText(pref + (amount).ToString());
        risingTextObj.GetComponent<Text>().color = Color.white;
        risingTextObj.transform.localPosition = popUpPos;
    }

    private void UpdateEnergy()
    {
        textEnergy.text = prefix + energy.ToString();
        // Keep track of peak energy for Analytics.
        if (energy > peakEnergy)
        {
            peakEnergy = energy;
        }
        // Update the energy meter.
        //energyMeter.UpdateEnergyMeter();

        // Check if the player is about to lose.
        int energyDrain = grid.GetEnergyDrain();
        SetIsAboutToLose(energyDrain >= energy);

        UpdateEnergyLevel();
    }

    private void SetIsAboutToLose(bool isAboutToLose)
    {
        if (isAboutToLose)
        {
            //Debug.Log("The player is about to lose!");
            AudioController.Instance.PlayLoop("About_To_Lose_1");
            TutorialController.Instance.TriggerEvent(TutorialController.Triggers.FIRST_LOW_ENERGY);
        }
        else
        {
            AudioController.Instance.StopSFX("About_To_Lose_1");
            //Debug.Log("The player isn't about to lose.");
        }
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