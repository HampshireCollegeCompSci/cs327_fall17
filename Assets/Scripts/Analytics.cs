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
    [Tooltip("Reference to the EnergyCounter instance")]
    EnergyCounter Energy;
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
        UnityEngine.Analytics.Analytics.CustomEvent("gameOver", new Dictionary<string, object>
        {
            { "score", score },
            { "gameOverCause", cause },
            { "peakEnergy", peakEnergy}
  });
    }
}
