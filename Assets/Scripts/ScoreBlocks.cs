// Author(s): Yixiang Xu, Paul Calande
// Class that controls the score blocks at the top of the screen.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBlocks : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The array of score blocks.")]
    Image[] scoreBlocks;

    [SerializeField]
    [Tooltip("The array of event blocks.")]
    Image[] eventBlocks;

    [SerializeField]
    [Tooltip("The sprite of empty score block.")]
    Sprite emptyScoreBlock;
    [SerializeField]
    [Tooltip("The sprite of full score block.")]
    Sprite fullScoreBlock;

    [SerializeField]
    [Tooltip("The sprite of empty event block.")]
    Sprite emptyEventBlock;
    [SerializeField]
    [Tooltip("The sprite of full event block.")]
    Sprite fullEventBlock;

    private int scoreBlockCount;
    private int eventBlockCount;

    private void Start()
    {
        scoreBlockCount = 0;
        eventBlockCount = 0;
    }

    public void ScoreBlockAdded()
    {
        if (scoreBlockCount < scoreBlocks.Length)
        {
            scoreBlocks[scoreBlockCount].GetComponent<Animator>().enabled = true;
            scoreBlocks[scoreBlockCount].sprite = fullScoreBlock;
            scoreBlockCount++;
        }
        else
        {
            eventBlocks[eventBlockCount].GetComponent<Animator>().enabled = true;
            eventBlocks[eventBlockCount].sprite = fullEventBlock;
            eventBlockCount++;
        }
        

        if (eventBlockCount == eventBlocks.Length)
        {
            scoreBlockCount -= scoreBlocks.Length;
            eventBlockCount -= eventBlocks.Length;

            foreach (Image i in scoreBlocks)
            {
                i.sprite = emptyScoreBlock;
            }

            foreach (Image i in eventBlocks)
            {
                i.sprite = emptyEventBlock;
            }
        }
    }
}