// Author(s): Joel Esquilin, Paul Calande, Wm. Josiah Erikson, Yixiang Xu, Maia

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameOver : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Reference to the game over window.")]
    GameObject gameOverWindow;
    [SerializeField]
    [Tooltip("Reference to the GameFlow instance.")]
    GameFlow gameFlow;
    /*
    [SerializeField]
    [Tooltip("An array of all the GameObjects to be enabled upon game over.")]
    GameObject[] toBeEnabled;
    */
    [SerializeField]
    [Tooltip("Reference to the Text object that will explain why the player lost.")]
    Text textGameOverReason;
	[SerializeField]
	[Tooltip("Reference to the Score Counter instance.")]
	ScoreCounter score;
    [SerializeField]
    [Tooltip("Reference to the Analytics instance.")]
    Analytics analytics;
    [SerializeField]
    [Tooltip("Reference to the Text object that will display the player's high score")]
    Text textHighScore;
    [SerializeField]
    [Tooltip("Reference to the Text object that will display the player's score")]
    Text textScore;
    [SerializeField]
    [Tooltip("Reference to the Text object that will display the literal text High Score, if English. Still needs to be implemented in the JSON.")]
    Text textHighScoreLabel;
    [SerializeField]
    [Tooltip("Reference to the Text object that will display the literal text Your Score, if English. Still needs to be implemented in the JSON.")]
    Text textYourScoreLabel;
    [SerializeField]
    [Tooltip("Reference to the Text object that will display the literal text Game Over, if English. Still needs to be implemented in the JSON.")]
    Text textGameOverLabel;
    [SerializeField]
    [Tooltip("Reference to the Text object that describes how far you've come.")]
    Text textYouReached;
    [SerializeField]
    [Tooltip("Reference to the VoidEventController.")]
    VoidEventController voidEventController;
    [SerializeField]
    [Tooltip("Reference to the progress bar overlay.")]
    RectTransform progressBarTop;
    [SerializeField]
    [Tooltip("The game over music to play.")]
    AudioClip musicGameOver;

    // Reference to the Translator.
	UILanguages translator;

    private void Awake()
    {
        translator = FindObjectOfType<UILanguages>();

        //if (gameFlow != null)
        //{
        gameFlow.GameLost += Appear;
        //}

        /*
        foreach (GameObject obj in toBeEnabled)
        {
            obj.SetActive(false);
        }
        */
        gameOverWindow.SetActive(false);
    }

    private void OnDestroy()
    {
        if (gameFlow != null)
        {
            gameFlow.GameLost -= Appear;
        }
    }

    // Callback function for GameFlow.GameLost.
    void Appear(GameFlow.GameOverCause cause)
    {
        AudioController.Instance.StopSFX("About_To_Lose_1");
        AudioController.Instance.GameOver();
        AudioController.Instance.PlayMusic(musicGameOver);
        /*
        foreach (GameObject obj in toBeEnabled)
        {
            obj.SetActive(true);
        }
        */
        gameOverWindow.SetActive(true);

        string reason;
        switch (cause)
        {
            case GameFlow.GameOverCause.NoRemainingSpaces:
                reason = "ReasonNoSpaceLeft";
                break;
            case GameFlow.GameOverCause.QueueOverflow:
                reason = "ReasonHesitation";
                break;
            case GameFlow.GameOverCause.NoMoreEnergy:
                reason = "ReasonNoEnergy";
                break;
            case GameFlow.GameOverCause.Reset:
                reason = "ReasonManualReset";
                break;
            default:
                reason = "ReasonHesitation";
                break;
        }

        int highScore = Settings.Instance.GetHighScore();
        int finalScore = score.GetScore();
        if (finalScore > highScore)
        {
            highScore = finalScore;
            Settings.Instance.SaveHighScore(highScore);
        }

        analytics.SendData(cause, highScore);

        textGameOverReason.text = translator.Translate(reason);
        textHighScore.text = highScore.ToString();
        textScore.text = score.GetScore().ToString();
        // For Maia - leaving this in here as a template
        textGameOverLabel.text = translator.Translate("GameOver");
        textHighScoreLabel.text = translator.Translate("HighScore1");
        textYourScoreLabel.text = translator.Translate("YourScore");
        textYouReached.text = "YOU REACHED:\n" + voidEventController.GetLatestEventName();

        float progress = voidEventController.GetProgress();
        progressBarTop.anchorMax = new Vector2(progress, progressBarTop.anchorMax.y);
        //Debug.Log(progressBarTop.anchorMax);
    }
}