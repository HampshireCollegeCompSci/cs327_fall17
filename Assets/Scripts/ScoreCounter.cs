// Author(s): Paul Calande

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This class keeps track of the player's score and updates the UI accordingly.
public class ScoreCounter : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Reference to the Grid.")]
    Grid grid;
    [SerializeField]
    [Tooltip("Reference to the Score UI text.")]
    Text textScore;

    // The current score.
    int score = 0;

    private void Start()
    {
        grid.SquareFormed += Grid_SquareFormed;
        UpdateScore();
    }

    private void OnDestroy()
    {
        grid.SquareFormed -= Grid_SquareFormed;
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
        textScore.text = "Score: " + score;
    }

    private void Grid_SquareFormed(int size, Vector3 textPos)
    {
        AddScore(size * 100);
        GameObject spawnTextScoreObj = new GameObject();
        Text spawnTextScore = spawnTextScoreObj.AddComponent<Text>();
        spawnTextScore.text = (size * 100).ToString();
        spawnTextScore.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        spawnTextScore.fontSize = 90;
        spawnTextScore.alignment = TextAnchor.MiddleCenter;
        spawnTextScoreObj.transform.SetParent(GameObject.Find("Canvas").transform);
        spawnTextScoreObj.transform.localPosition = textPos;
        Instantiate(spawnTextScoreObj, spawnTextScoreObj.transform.localPosition, Quaternion.identity);
    }
}