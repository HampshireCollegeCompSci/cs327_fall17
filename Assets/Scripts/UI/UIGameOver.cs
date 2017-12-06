// Author(s): Joel Esquilin, Paul Calande, Wm. Josiah Erikson, Yixiang Xu, Maia

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;

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
    [Tooltip("Reference to the progress bar.")]
    ProgressBar progressBar;
    [SerializeField]
    [Tooltip("The game over music to play.")]
    AudioClip musicGameOver;
    [SerializeField]
    [Tooltip("Reference to the console mask.")]
    GameObject maskConsole;
    [SerializeField]
    [Tooltip("Reference to the reactor mask.")]
    GameObject maskReactor;
    [SerializeField]
    [Tooltip("How long the game over window waits to appear when game over status is achieved. Populated by JSON.")]
    float secondsToWait;
    [SerializeField]
    [Tooltip("Reference to the tuning JSON.")]
    TextAsset tuningJSON;

    // Whether the game over wait time shall be incremented.
    bool waitingForGameOver = false;
    // How long the game over has been waited for since the last call to ResetGameOverWaitTime.
    float secondsWaitedForGameOver = 0.0f;
    // The cause of the game over.
    GameFlow.GameOverCause causeOfGameOver;
    // Reference to the Translator.
    UILanguages translator;
    // Whether the game is over or not.
    bool gameIsOver = false;

    private void Awake()
    {
        translator = FindObjectOfType<UILanguages>();

        gameFlow.GameLost += GameFlow_GameLost;

        gameOverWindow.SetActive(false);

        Tune();
    }

    private void Tune()
    {
        JSONNode json = JSON.Parse(tuningJSON.ToString());
        secondsToWait = json["seconds to wait for game over"].AsFloat;
    }

    private void OnDestroy()
    {
        if (gameFlow != null)
        {
            gameFlow.GameLost -= GameFlow_GameLost;
        }
    }

    public void TryStartGameOverWaitTimer()
    {
        if (gameIsOver)
        {
            StartGameOverWaitTimer();
        }
    }

    public void StartGameOverWaitTimer()
    {
        waitingForGameOver = true;
    }

    public void ResetGameOverWaitTime()
    {
        secondsWaitedForGameOver = 0.0f;
    }

    private void Update()
    {
        if (waitingForGameOver)
        {
            secondsWaitedForGameOver += Time.deltaTime;
            if (secondsWaitedForGameOver > secondsToWait)
            {
                waitingForGameOver = false;

                maskConsole.SetActive(false);
                maskReactor.SetActive(false);

                Appear(causeOfGameOver);
            }
        }
    }

    // Callback function for GameFlow.GameLost.
    void GameFlow_GameLost(GameFlow.GameOverCause cause)
    {
        AudioController.Instance.StopSFX("About_To_Lose_1");
        AudioController.Instance.GameOver();

        float waitTime = secondsToWait;
        switch (cause)
        {
            case GameFlow.GameOverCause.NoRemainingSpaces:
                maskConsole.SetActive(true);
                // Wait a little longer than usual.
                waitTime += 2.0f;
                break;

            case GameFlow.GameOverCause.NoMoreEnergy:
                // BlockSpawner handles its game over logic itself.
                maskReactor.SetActive(true);
                break;
        }
        secondsToWait = waitTime;

        if (cause == GameFlow.GameOverCause.NoRemainingSpaces)
        {
            StartGameOverWaitTimer();
        }

        causeOfGameOver = cause;
        gameIsOver = true;
    }

    void Appear(GameFlow.GameOverCause cause)
    {
        AudioController.Instance.PlayMusic(musicGameOver);
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
        //textGameOverLabel.text = translator.Translate("GameOver");
        //textHighScoreLabel.text = translator.Translate("HighScore1");
        //textYourScoreLabel.text = translator.Translate("YourScore");
        textYouReached.text = "YOU REACHED:\n" + voidEventController.GetLatestEventName();
		/*
		if (translator.IsLanguageThatNeedsNewFont == true) {
			textGameOverReason.trueTypeFont = translator.Font ();
			textGameOverLabel.trueTypeFont = translator.Font ();
			textHighScoreLabel.trueTypeFont = translator.Font ();
			textYourScoreLabel.trueTypeFont = translator.Font ();
		}
		*/
        float progress = voidEventController.GetProgress();
        progressBar.InstantiateEvents(voidEventController.GetVoidEventGroups());
        progressBar.SetProgress(progress);
        //Debug.Log(progressBarTop.anchorMax);
    }
}