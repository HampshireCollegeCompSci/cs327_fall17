﻿//Author: Wm. Josiah Erikson
//This is the class that will subscribe to the GameLost event and send data up to Unity's servers at that time
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// Reference the Unity Analytics namespace
using UnityEngine.Analytics;
using System;

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

    Settings settings;

    // Use this for initialization
    void Awake()
    {
        settings = FindObjectOfType<Settings>(); //Can't pass in a reference because it's a singleton
    }

    public void SendData(GameFlow.GameOverCause cause, int highScore)
    {
        if (settings.AreCheatsEnabled())
        {
            //Debug.Log("Not sending analytics because cheats are enabled.");
            return; //Don't send analytics if cheats were enabled
        }
        //Debug.Log("Sending analytics!");
        int finalScore = score.GetScore();
        int peakEnergy = energy.GetPeakEnergy();
        int turnsPlayed = turns.GetTurns();
        int peakVestiges = vestiges.GetPeakVestiges();
        int currentVestiges = vestiges.GetCurrentVestiges();
        int finalClearedSquares = clearedSquares.GetClearedSquares();
        float timePlaying = Time.time;
        bool isZenMode = settings.IsZenModeEnabled();
        //string currentTimeStamp = System.DateTime.Now.ToString();
        //string allTheData = "timestamp;" + currentTimeStamp + ",score;" + finalScore + ",peakEnergy;" + peakEnergy + ",timePlaying;" + timePlaying + ",turnsPlayed;" + turnsPlayed + ",peakVestiges;" + peakVestiges + ",endingVestiges;" + currentVestiges + ",totalClearedSquares;" + finalClearedSquares;
        //Debug.Log("allTheData is" + allTheData);
        UnityEngine.Analytics.Analytics.CustomEvent("gameOver", new Dictionary<string, object>
        {
            { "score", finalScore },
            { "gameOverCause", cause },
            { "peakEnergy", peakEnergy},
            { "timePlaying", timePlaying},
            { "turnsPlayed", turnsPlayed},
            { "peakVestiges", peakVestiges},
            { "endingVestiges", currentVestiges},
            { "totalClearedSquares", finalClearedSquares},
            { "highScore", highScore},
            { "isZenMode", isZenMode }
            //{ "allTheData", allTheData} //I removed this because it wasn't working anyway

        });
    }
}
