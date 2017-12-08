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
    ///private bool blocksCleared;

    private void Start()
    {
        scoreBlockCount = 0;
        eventBlockCount = 0;
        ///blocksCleared = true;
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
            ///blocksCleared = false;
            scoreBlockCount -= scoreBlocks.Length;
            eventBlockCount -= eventBlocks.Length;
            StartCoroutine(ClearingFullBlocks());
        }
        
    }

    private IEnumerator ClearingFullBlocks()
    {
        yield return new WaitUntil(() => eventBlocks[eventBlocks.Length - 1].GetComponent<Animator>().enabled == false || scoreBlocks[0].GetComponent<Animator>().enabled == true);
        
        foreach (Image i in scoreBlocks)
        {
            i.sprite = emptyScoreBlock;
        }

        foreach (Image i in eventBlocks)
        {
            i.GetComponent<Animator>().enabled = false;
            i.sprite = emptyEventBlock;
        }

        ///blocksCleared = true;
    }
}