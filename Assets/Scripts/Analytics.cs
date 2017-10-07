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
    ScoreCounter Score;
    [SerializeField]
    [Tooltip("Reference to the EnergyCounter instance.")]
    EnergyCounter Energy;
    [SerializeField]
    [Tooltip("Reference to the TurnsCounter instance.")]
    TurnCounter Turns;
    [SerializeField]
    [Tooltip("Reference to the VestigeCounter instance.")]
    VestigeCounter Vestiges;
    [SerializeField]
    [Tooltip("Reference to the ClearedSquares instance.")]
    ClearedSquaresCounter ClearedSquares;
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
        int score = Score.GetScore();
        int peakEnergy = Energy.GetPeakEnergy();
        int turnsPlayed = Turns.GetTurns();
        int peakVestiges = Vestiges.GetPeakVestiges();
        int currentVestiges = Vestiges.GetCurrentVestiges();
        int clearedSquares = ClearedSquares.GetClearedSquares();
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
