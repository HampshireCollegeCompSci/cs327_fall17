// Author(s): Paul Calande, Wm. Josiah Erikson

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This class keeps track of the player's score and updates the UI accordingly.
public class ScoreCounter : MonoBehaviour
{
    // Invoked when the score changes, with the new score passed in as an argument.
    public delegate void ScoreChangedHandler(int newScore);
    public event ScoreChangedHandler ScoreChanged;

    [SerializeField]
    [Tooltip("Reference to the Grid instance.")]
    Grid grid;
    [SerializeField]
    [Tooltip("Reference to the Score UI text.")]
    Text textScore;
    [SerializeField]
    [Tooltip("The Rising Text prefab to instantiate when points are gained from cleared squares.")]
    GameObject risingTextPrefab;

	UILanguages translator;

    // The current score.
    int score = 0;

    private void Start()
    {
	translator = FindObjectOfType<UILanguages>();
        grid.SquareCleared += Grid_SquareCleared;
        UpdateScore();
    }

    public int GetScore()
    {
        return score;
    }

    private void OnDestroy()
    {
        grid.SquareCleared -= Grid_SquareCleared;
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScore();
    }

    public void RemoveScore(int amount)
    {
        score -= amount;
        UpdateScore();
    }

    private void UpdateScore()
    {
        textScore.text = translator.Translate("Score") + score;
        //textScore.text = "Score: " + score;
        OnScoreChanged();
    }

    // Callback function for a square on the Grid being cleared.
    private void Grid_SquareCleared(int scorePerSquare, Vector3 textPos)
    {
        AddScore(scorePerSquare);

        // Instantiate some rising text.
        GameObject risingTextObj = Object.Instantiate(risingTextPrefab, grid.transform, false);
        risingTextObj.GetComponent<RisingText>().SetText(scorePerSquare.ToString());
        risingTextObj.transform.localPosition = textPos;
    }

    private void OnScoreChanged()
    {
        if (ScoreChanged != null)
        {
            ScoreChanged(score);
        }
    }
}
