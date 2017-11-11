// Author(s): Yixiang Xu

using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UIYouWin : MonoBehaviour {
    [SerializeField]
    [Tooltip("Reference to the high score UI text.")]
    Text highScore;
    [SerializeField]
    [Tooltip("Reference to the your score UI text.")]
    Text yourScore;
    [SerializeField]
    [Tooltip("Reference to the score counter.")]
    ScoreCounter scoreCounter;

    public void init()
    {
        yourScore.text = scoreCounter.GetScore().ToString();

        string prefsEntry;

        if (Settings.Instance.IsZenModeEnabled())
        {
            prefsEntry = "HighScoreZen";
        }
        else
        {
            prefsEntry = "HighScore";
        }

        highScore.text = PlayerPrefs.GetInt(prefsEntry).ToString();
    }

    public void Resume()
    {
        gameObject.SetActive(false);
        AudioController.Instance.MenuClick();
    }
}
