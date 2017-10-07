//Author: Wm. Josiah Erikson
//This is the class that will subscribe to the GameLost event and send data up to Unity's servers at that time
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// Reference the Unity Analytics namespace
using UnityEngine.Analytics;

public class Analytics : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Reference to the GameFlow instance.")]
    GameFlow gameFlow;
    [SerializeField]
    [Tooltip("Reference to the Score instance.")]
    ScoreCounter score;
    [SerializeField]
    [Tooltip("Reference to the EnergyCounter instance.")]
    EnergyCounter energy;
    [SerializeField]
    [Tooltip("Reference to the TurnsCounter instance.")]
    TurnCounter turns;
    [SerializeField]
    [Tooltip("Reference to the VestigeCounter instance.")]
    VestigeCounter vestiges;
    [SerializeField]
    [Tooltip("Reference to the ClearedSquares instance.")]
    ClearedSquaresCounter clearedSquares;
    // Use this for initialization
    void Start()
    {
        if (gameFlow != null)
        {
            gameFlow.GameLost += SendData;
        }

    }

    void SendData(GameFlow.GameOverCause cause)
    {
        //Debug.Log("Sending analytics!");
        int score = score.GetScore();
        int peakEnergy = energy.GetPeakEnergy();
        int turnsPlayed = turns.GetTurns();
        int peakVestiges = vestiges.GetPeakVestiges();
        int currentVestiges = vestiges.GetCurrentVestiges();
        int clearedSquares = clearedSquares.GetClearedSquares();
        UnityEngine.Analytics.Analytics.CustomEvent("gameOver", new Dictionary<string, object>
        {
            { "score", score },
            { "gameOverCause", cause },
            { "peakEnergy", peakEnergy},
            { "timePlaying", Time.time},
            { "turnsPlayed", turnsPlayed},
            { "peakVestiges", peakVestiges},
            { "endingVestiges", currentVestiges},
            { "clearedSquares", clearedSquares}

  });
    }
}
