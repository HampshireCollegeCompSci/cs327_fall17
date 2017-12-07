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
    public enum TextColor
    {
        Gain,
        Vestige1,
        Vestige2,
        Vestige3
    }

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
    [Tooltip("Reference to energy gain animator controller.")]
    Animator energyGainController;
    [SerializeField]
    [Tooltip("The color that the rising text should have when energy is gained.")]
    Color energyTextColorGain;
    [SerializeField]
    [Tooltip("The color that the rising text should have when energy is lost.")]
    Color energyTextColorLoss;
    [SerializeField]
    [Tooltip("The color that the rising text should have when energy is lost from a level 1 vestige.")]
    Color energyTextColorVestige1;
    [SerializeField]
    [Tooltip("The color that the rising text should have when energy is lost from a level 2 vestige.")]
    Color energyTextColorVestige2;
    [SerializeField]
    [Tooltip("The color that the rising text should have when energy is lost from a level 3 vestige.")]
    Color energyTextColorVestige3;
    [SerializeField]
    [Tooltip("Reference to the UI canvas transform.")]
    Transform canvas;
    [SerializeField]
    [Tooltip("Reference to energy transfer ball animator controller.")]
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
    /*
    [SerializeField]
    [Tooltip("The text color to use for the energy counter when the player isn't at the lowest energy threshold.")]
    Color energyTextColorNormal;
    [SerializeField]
    [Tooltip("The text color to use for the energy counter when the player is at the lowest energy threshold.")]
    Color energyTextColorDanger;
    */
    [SerializeField]
    [Tooltip("Prefab for the rising text accumulator.")]
    GameObject prefabRisingTextAccumulator;
    [SerializeField]
    [Tooltip("The accumulator's desired transform.")]
    Transform accumulatorTransform;
    [SerializeField]
    [Tooltip("Transform corresponding to the reactor center.")]
    Transform reactorCenterTransform;

    // The highest amount of energy achieved over the course of the game.
    int peakEnergy;

    // The reference to block transform.
    Transform blockTransform;

    // Reference to the accumulation text.
    RisingTextAccumulator accumulator = null;

    float accumulationSecondsToWait;
    float accumulationSecondsToTravel;

    static int hashActivate = Animator.StringToHash("activate");

    void Tune()
    {
        var json = JSON.Parse(tuningJSON.ToString());
        energy = json["starting energy"].AsInt;
        //maxEnergyInMeter = json["max energy in meter"].AsInt;
        energyThreshold = json["energy level animation turns remaining"].AsArray;
        textSecondsBeforeHeadingToDestination = json["seconds for rising text to rise"].AsFloat;
        accumulationSecondsToWait = json["accumulation seconds to wait"].AsFloat;
        accumulationSecondsToTravel = json["accumulation seconds to travel"].AsFloat;
    }

    void Start()
    {
        Tune();

        //energyTextColorNormal = energyTextColorGain;
        //energyTextColorDanger = energyTextColorLoss;

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
            textEnergy.color = energyTextColorLoss;

            EnterEnergyLevel(0);
            return;
        }
        else
        {
            imageConsoleGrid.sprite = spriteConsoleGridNormal;
            textEnergy.color = energyTextColorGain;
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
            energyTransferBallController.SetTrigger(hashActivate);
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

    // Creates a text popup.
    // popUpPos should be world position, not local position.
    public void PopUp(int amount, Vector3 popUpPos, TextColor colIn, Vector3? destinationPos = null)
    {
        GameObject risingTextObj = Instantiate(risingTextPrefab, canvas, false);

        /*
        Debug.Log("EnergyCounter.PopUp: amount / popUpPos / destinationPos: "
    + amount + " / " + popUpPos + " / " + destinationPos);
        Debug.Log("Is risingTextObj null: " + (risingTextObj == null));
        Debug.Log("risingTextObj: " + risingTextObj);
        */

        string pref;
        if (amount < 0)
        {
            // The number is negative already, so we don't need a prefix.
            pref = "";
        }
        else
        {
            pref = "+";
        }
        Color col;
        switch (colIn)
        {
            default:
            case TextColor.Gain:
                col = energyTextColorGain;
                break;
            case TextColor.Vestige1:
                col = energyTextColorVestige1;
                break;
            case TextColor.Vestige2:
                col = energyTextColorVestige2;
                break;
            case TextColor.Vestige3:
                col = energyTextColorVestige3;
                break;
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

            if (amount > 0)
            {
                risingTextObj.GetComponent<LerpTo>().Completed +=
                    (() => RisingTextEnergyPenaltyCallback(amount));
            }
            else
            {
                risingTextObj.GetComponent<LerpTo>().Completed +=
                    (() => RisingTextEnergyPenaltyAccumulateCallback(amount));
            }
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
        /*
        if (amount < 0)
        {
            grid.AnimateReactorEnergyLoss();
            gameOver.ResetGameOverWaitTime();
        }
        */
    }

    private void RisingTextEnergyPenaltyAccumulateCallback(int amount)
    {
        // If the accumulator doesn't exist, instantiate it.
        if (accumulator == null)
        {
            GameObject acc = Instantiate(prefabRisingTextAccumulator, canvas, false);
            acc.transform.position = accumulatorTransform.position;

            accumulator = acc.GetComponent<RisingTextAccumulator>();
            accumulator.Init();
            accumulator.SetReactorCenter(reactorCenterTransform);
            accumulator.SetColor(energyTextColorLoss);
            accumulator.SetSecondsToWait(accumulationSecondsToWait);
            accumulator.SetSecondsToTravel(accumulationSecondsToTravel);
            accumulator.MoveStarted += RisingTextAccumulator_MoveStarted;
            accumulator.Completed += RisingTextAccumulator_Completed;
        }
        accumulator.Add(amount);
        accumulator.ResetTimer();
    }

    private void RisingTextAccumulator_MoveStarted()
    {
        accumulator = null;
    }

    private void RisingTextAccumulator_Completed(int amount)
    {
        AddToBufferedEnergyTarget(amount);
        grid.AnimateReactorEnergyLoss();
        //gameOver.ResetGameOverWaitTime();
        gameOver.TryStartGameOverWaitTimer();
    }
}