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
    [SerializeField]
    [Tooltip("Reference to the score blocks parent, which will be disabled.")]
    GameObject scoreBlocks;

    public void Init()
    {
        int theScore = scoreCounter.GetSquaresCleared();
        yourScore.text = theScore.ToString();
        int theHighScore = Settings.Instance.GetHighScore();
        if (theScore > theHighScore)
        {
            highScore.text = theScore.ToString();
        }
        else
        {
            highScore.text = theHighScore.ToString();
        }
    }

    public void Resume()
    {
        gameObject.SetActive(false);
        AudioController.Instance.MenuClick();
    }

    private void OnEnable()
    {
        AudioController.Instance.WinGame();
        scoreBlocks.SetActive(false);
    }
}