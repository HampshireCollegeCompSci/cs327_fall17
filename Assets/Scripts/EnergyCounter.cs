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
    [SerializeField]
    [Tooltip("The color that the rising text should have when energy is gained.")]
    Color energyTextColorGain;
    [SerializeField]
    [Tooltip("The color that the rising text should have when energy is lost.")]
    Color energyTextColorLoss;
    [SerializeField]
    [Tooltip("Reference to the UI canvas transform.")]
    Transform canvas;
    [SerializeField]
    [Tooltip("Reference to energy transfer ball animator.")]
    Animator energyTransferBallController;
    [SerializeField]
    [Tooltip("Reference to the BufferedValue component for buffering the energy.")]
    BufferedValue bufferedValueEnergy;
    [SerializeField]
    [Tooltip("Console Grid sprite to use when the player isn't at the lowest energy threshold.")]
    Sprite spriteConsoleGridNormal;
    [SerializeField]
    [Tooltip("Console Grid sprite to use when the player is at the lowest energy threshold.")]
    Sprite spriteConsoleGridDanger;
    [SerializeField]
    [Tooltip("Reference to the console grid image.")]
    Image imageConsoleGrid;
    [SerializeField]
    [Tooltip("Reference to the game over controller.")]
    UIGameOver gameOver;
    [SerializeField]
    [Tooltip("How many seconds the rising text will wait before heading to its destination. Populated by JSON.")]
    float textSecondsBeforeHeadingToDestination;
    [SerializeField]
    [Tooltip("The text color to use for the energy counter when the player isn't at the lowest energy threshold.")]
    Color energyTextColorNormal;
    [SerializeField]
    [Tooltip("The text color to use for the energy counter when the player is at the lowest energy threshold.")]
    Color energyTextColorDanger;

    // The highest amount of energy achieved over the course of the game.
    int peakEnergy;

    // The reference to block transform.
    Transform blockTransform;

    void Tune()
    {
        var json = JSON.Parse(tuningJSON.ToString());
        energy = json["starting energy"].AsInt;
        //maxEnergyInMeter = json["max energy in meter"].AsInt;
        energyThreshold = json["energy level animation turns remaining"].AsArray;
        textSecondsBeforeHeadingToDestination = json["seconds for rising text to rise"].AsFloat;
    }

    void Start()
    {
        Tune();

        energyTextColorNormal = energyTextColorGain;
        energyTextColorDanger = energyTextColorLoss;

        bufferedValueEnergy.ValueUpdated += BufferedValueEnergy_ValueUpdated;
        bufferedValueEnergy.ForceSetBufferedValue(energy);
        //SetEnergyLevel();
        UpdateEnergy();
    }

    private void EnterEnergyLevel(int level)
    {
        energyGainController.SetInteger("energyLevel", level);
    }

    // Updates the animation on the reactor.
    private void UpdateEnergyLevel(int turnsRemaining)
    {
        if (turnsRemaining <= energyThreshold[0])
        {
            imageConsoleGrid.sprite = spriteConsoleGridDanger;
            textEnergy.color = energyTextColorDanger;

            EnterEnergyLevel(0);
            return;
        }
        else
        {
            imageConsoleGrid.sprite = spriteConsoleGridNormal;
            textEnergy.color = energyTextColorNormal;
        }

        if (turnsRemaining <= energyThreshold[1])
        {
            EnterEnergyLevel(1);
            return;
        }

        if (turnsRemaining <= energyThreshold[2])
        {
            EnterEnergyLevel(2);
            return;
        }

        EnterEnergyLevel(3);
    }

    // Sets the energy level based on the highest threshold.
    public void EnterHighestEnergyLevel()
    {
        UpdateEnergyLevel(energyThreshold[energyThreshold.Count - 1]);
    }

    public void AddEnergy(int amount, bool updateBufferedEnergyTarget = true)
    {
        if (amount < -energy)
        {
            amount = -energy;
        }
        energy += amount;
        /*
        if (energy > maxEnergy)
        {
            energy = maxEnergy;
        }
        */
        if (energy == 0)
        {
            gameFlow.GameOver(GameFlow.GameOverCause.NoMoreEnergy);
        }

        if (updateBufferedEnergyTarget)
        {
            AddToBufferedEnergyTarget(amount);
        }

        UpdateEnergy();

        if (amount > 0)
        {
            // Activate the energy transfer ball animation.
            energyTransferBallController.SetBool("active", true);
        }
    }

    public void RemoveEnergy(int amount, bool updateBufferedEnergyTarget = true)
    {
        AddEnergy(-amount, updateBufferedEnergyTarget);
    }

    public void SetEnergy(int newEnergy, bool updateBufferedEnergyTarget = true)
    {
        AddEnergy(newEnergy - energy, updateBufferedEnergyTarget);
    }

    public void PopUp(int amount, Vector3 popUpPos, Vector3? destinationPos = null)
    {
        GameObject risingTextObj = Instantiate(risingTextPrefab, canvas, false);

        /*
        Debug.Log("EnergyCounter.PopUp: amount / popUpPos / destinationPos: "
    + amount + " / " + popUpPos + " / " + destinationPos);
        Debug.Log("Is risingTextObj null: " + (risingTextObj == null));
        Debug.Log("risingTextObj: " + risingTextObj);
        */

        string pref;
        Color col;
        if (amount < 0)
        {
            // The number is negative already, so we don't need a prefix.
            pref = "";
            col = energyTextColorLoss;
        }
        else
        {
            pref = "+";
            col = energyTextColorGain;
        }
        RisingText risingText = risingTextObj.GetComponent<RisingText>();
        risingText.SetText(pref + amount.ToString());
        risingText.SetColor(col);
        risingTextObj.transform.position = popUpPos;
        if (destinationPos != null)
        {
            //Debug.Log("EnergyCounter.PopUp: Destination position is not null!");
            // Prepare to move to the given destination.
            risingText.SetDestination((Vector3)destinationPos);
            risingText.SetSecondsBeforeHeadingToDestination(textSecondsBeforeHeadingToDestination);
            //risingTextObj.GetComponent<LerpTo>().Completed += RisingTextEnergyPenaltyCallback;
            risingTextObj.GetComponent<LerpTo>().Completed +=
                (() => RisingTextEnergyPenaltyCallback(amount));
        }
    }

    private void UpdateEnergy()
    {
        // Keep track of peak energy for Analytics.
        if (energy > peakEnergy)
        {
            peakEnergy = energy;
        }

        // Check if the player is about to lose.
        int energyDrain = grid.GetEnergyDrain();
        if (energyDrain <= 0)
        {
            SetIsAboutToLose(false);
            EnterHighestEnergyLevel();
        }
        else
        {
            int turnsRemaining = Mathf.CeilToInt((float)energy / energyDrain);
            SetIsAboutToLose(turnsRemaining == 1);
            UpdateEnergyLevel(turnsRemaining);
        }
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
            //Debug.Log("The player isn't about to lose.");
            AudioController.Instance.StopSFX("About_To_Lose_1");
        }
    }

    public int GetPeakEnergy()
    {
        return peakEnergy;
    }

    public int GetCurrentEnergy()
    {
        return energy;
    }

    public void AddToBufferedEnergyTarget(int amount)
    {
        bufferedValueEnergy.AddToBufferedValueTarget(amount);
    }

    private void RisingTextEnergyPenaltyCallback(int amount)
    {
        //Debug.Log("EnergyCounter.RisingTextEnergyPenaltyCallback");
        AddToBufferedEnergyTarget(amount);
        if (amount < 0)
        {
            grid.AnimateReactorEnergyLoss();
            gameOver.ResetGameOverWaitTime();
        }
    }

    private void BufferedValueEnergy_ValueUpdated(int newValue, int difference)
    {
        //Debug.Log("EnergyCounter.BufferedValueEnergy_ValueUpdated.newValue: " + newValue);
        textEnergy.text = newValue.ToString();
    }
}