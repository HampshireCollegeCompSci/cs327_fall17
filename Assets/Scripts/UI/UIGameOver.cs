// Author(s): Joel Esquilin, Paul Calande, Wm. Josiah Erikson, Yixiang Xu

using System;
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
    [SerializeField]
    [Tooltip("Reference to the Score Counter instance.")]
    ScoreCounter score;
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
        AudioController.Instance.MenuClick();

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    // Callback function for GameFlow.GameLost.
    void Appear(GameFlow.GameOverCause cause)
    {
        AudioController.Instance.StopSFX("About_To_Lose_1");
        AudioController.Instance.PlaySFX("Game_Over_1");

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
            case GameFlow.GameOverCause.Reset:
                reason = "Manual reset!";
                break;
            default:
                reason = "Unknown reason. Please inform the programming team.";
                break;
        }
        int highScore = PlayerPrefs.GetInt("HighScore"); //Get the stored high score - 0 if doesn't exist
        int finalScore = score.GetScore();
        if (finalScore > highScore)
        {
            highScore = finalScore;
        }

        textGameOverReason.text = "Reason: " + reason + " High Score: " + highScore;
    }
}