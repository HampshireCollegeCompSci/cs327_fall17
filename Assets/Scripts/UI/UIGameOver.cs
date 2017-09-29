// Author(s): Joel Esquilin, Paul Calande

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIGameOver : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Reference to the GameFlow instance.")]
    GameFlow gameFlow;
    [SerializeField]
    [Tooltip("An array of all the GameObjects to be enabled upon game over.")]
    GameObject[] toBeEnabled;
    [SerializeField]
    [Tooltip("Reference to the Text object that will explain why the player lost.")]
    Text textGameOverReason; 

    private void Start()
    {
        if (gameFlow != null)
        {
            gameFlow.GameLost += Appear;
        }

        foreach (GameObject obj in toBeEnabled)
        {
            obj.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (gameFlow != null)
        {
            gameFlow.GameLost -= Appear;
        }
    }

    public void Reset()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    // Callback function for GameFlow.GameLost.
    void Appear(GameFlow.GameOverCause cause)
    {
        foreach (GameObject obj in toBeEnabled)
        {
            obj.SetActive(true);
        }
        string reason;
        switch (cause)
        {
            case GameFlow.GameOverCause.NoRemainingSpaces:
                reason = "No space left!";
                break;
            case GameFlow.GameOverCause.QueueOverflow:
                reason = "You hesitated for too long!";
                break;
            case GameFlow.GameOverCause.NoMoreEnergy:
                reason = "Out of energy!";
                break;
            default:
                reason = "Unknown reason. Please inform the programming team.";
                break;
        }
        textGameOverReason.text = "Reason: " + reason;
    }
}