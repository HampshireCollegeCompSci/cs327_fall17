using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBlocks : MonoBehaviour {
    [SerializeField]
    [Tooltip("The array of blocks.")]
    Image[] blocks;

    [SerializeField]
    [Tooltip("The sprite of empty score block.")]
    Sprite emptyScoreBlock;
    [SerializeField]
    [Tooltip("The sprite of full score block.")]
    Sprite fullScoreBlock;

    private int blockCount;

    private void Start()
    {
        blockCount = 0;
    }

    public void BlockAdded()
    {
        if(blockCount == blocks.Length)
        {
            blockCount = 0;
            foreach (Image i in blocks)
            {
                i.sprite = emptyScoreBlock;
            }

        }

        blocks[blockCount].sprite = fullScoreBlock;
        blockCount++;
    }
}
