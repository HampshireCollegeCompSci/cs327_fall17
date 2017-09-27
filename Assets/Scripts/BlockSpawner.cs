// Author(s): Paul Calande, Yifeng Shi
/* This class is working for spawning the random blocks
 * and maintaining the queue of incoming blocks.
 */
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    [SerializeField]
    float timeBetweenBlocks;
    [SerializeField]
    int maxBlocksInQueue;
    [SerializeField]
    GameObject[] blockPositions;
    [SerializeField]
    GameFlow gameFlow;
    [SerializeField]
    Grid grid;
    [SerializeField]
    GameObject prefabDraggableBlock;
    //[SerializeField]
    //File blockData;

    List<Block> possibleBlocks = new List<Block>();

    Queue<DraggableBlock> blocksQueue = new Queue<DraggableBlock>();

    float timeBeforeNextBlock;

    private void Start()
    {
        gameFlow.GameLost += GameFlow_GameLost;

        // Make sure that we can't have more DraggableBlocks in the queue
        // than there are block positions!
        if (maxBlocksInQueue > blockPositions.Length)
        {
            maxBlocksInQueue = blockPositions.Length;
        }
    }

    public void Init()
    {
        // TODO: Replace this for loop with file reading later.
        for (int i = 0; i < 10; ++i)
        {
            Block block;
            int formation = Random.Range(0, 3);
            switch (formation)
            {
                case 0:
                    block = new Block(2, 2);
                    break;
                case 1:
                    block = new Block(1, 2);
                    break;
                case 2:
                default:
                    block = new Block(2, 1);
                    break;
            }

            int w = block.GetWidth();
            int h = block.GetHeight();
            for (int row = 0; row < h; ++row)
            {
                for (int col = 0; col < w; ++col)
                {
                    int which = Random.Range(0, 3);
                    if (which < 2 && !((row == 0 && col == 0) || (row == h - 1 && col == w - 1)))
                    {
                        block.Fill(row, col, TileData.TileType.Unoccupied);
                    }
                    else
                    {
                        block.Fill(row, col, TileData.TileType.Regular);
                    }
                }
            }

            possibleBlocks.Add(block);
        }
    }

    private void Update()
    {
        //Decrement the time until next block to spawn.
        //If reaching 0, spawn a new random block and 
        //reset the timer
        if (timeBeforeNextBlock <= 0)
        {
            ResetBlockTimer();
            SpawnRandomBlock();
        }
            
        timeBeforeNextBlock -= Time.deltaTime;
    }

    public void ResetBlockTimer()
    {
        //Reset the timer for spawning next block
        timeBeforeNextBlock = timeBetweenBlocks;
    }

    public void SpawnRandomBlock()
    {
        if (!enabled)
        {
            // If the component is disabled, don't spawn a block.
            return;
        }
        if(blocksQueue.Count == maxBlocksInQueue)
        {
            //if the # of elements in queue already reaches
            //max, game over
            gameFlow.GameOver(GameFlow.GameOverCause.QueueOverflow);
            return;
        }
        else
        {
            //otherwise we select a random block from the possible list,
            //then instantiate the draggable block and add it into the queue.
            int i = Random.Range(0, possibleBlocks.Count);
            Block toSpawn = possibleBlocks[i];

            // Instantiate the actual block.
            GameObject newBlock = Instantiate(prefabDraggableBlock, transform, false);
            // Initialize the DraggableBlock component.
            DraggableBlock newDraggable = newBlock.GetComponent<DraggableBlock>();
            //newDraggable.Init(toSpawn, grid, canvas);
            newDraggable.Init(toSpawn, grid, GetComponent<RectTransform>());
            // Add it to the queue.
            blocksQueue.Enqueue(newDraggable);

            // If this block is the only block in queue, enable it.
            if (blocksQueue.Count == 1)
            {
                EnableFrontBlock();
            }
            else
            {
                // This block is not the only block in the queue.
                // Force the front block to be the last sibling so
                // that it is drawn above all of the other blocks.
                DraggableBlock frontBlock = blocksQueue.Peek();
                frontBlock.transform.SetAsLastSibling();
            }

            // Position new block at the next available blockPosition.
            int closestIndex = blocksQueue.Count - 1;
            PositionBlockAt(newDraggable, closestIndex);
            newDraggable.SetDefaultPosition(newBlock.transform.localPosition);
        }
    }

    public void UpdateAllBlocks()
    {
        foreach(DraggableBlock db in blocksQueue)
        {
            db.UpdateAvailableSpaces();
        }
    }

    public void ProgressQueue()
    {
        //Dequeue the block at the front
        blocksQueue.Dequeue();
        
        if (blocksQueue.Count == 0)
        {
            //if there is no blocks in the queue, spwan a 
            //new block now and reset the timer
            SpawnRandomBlock();
            ResetBlockTimer();
        }
        else
        {
            //Otherwise make the front block available
            EnableFrontBlock();
        }
        PositionAllBlocks();
    }

    void EnableFrontBlock()
    {
        //Check the size of queue just for safety
        if(blocksQueue.Count > 0)
        {
            // Set up the front block.
            DraggableBlock frontBlock = blocksQueue.Peek();
            frontBlock.AllowDragging(true);
            // If there is no more room for the Block on the Grid, game over.
            if (grid.CheckIfSpacesFilled(frontBlock.GetBlock()))
            {
                gameFlow.GameOver(GameFlow.GameOverCause.NoRemainingSpaces);
            }
        }
    }

    void PositionAllBlocks()
    {
        // Set each of the blocks in the queue
        // to its corresponding position.
        int i = 0;
        foreach (DraggableBlock db in blocksQueue)
        {
            PositionBlockAt(db, i);

            // We only need to set the default position for the
            // DraggableBlock at the front of the queue.
            if (i == 0)
            {
                db.SetDefaultPosition(db.transform.localPosition);
            }
            // Prepare for the next iteration of the loop.
            i++;
        }
    }

    void PositionBlockAt(DraggableBlock db, int positionIndex)
    {
        db.transform.localPosition = blockPositions[positionIndex].transform.localPosition;
    }

    public void RotateCurrentBlock(bool clockwise)
    {
        if (blocksQueue.Count > 0)
        {
            blocksQueue.Peek().Rotate(clockwise);
        }
    }

    // Callback function for gameFlow's GameLost event.
    private void GameFlow_GameLost(GameFlow.GameOverCause cause)
    {
        if (blocksQueue.Count > 0)
        {
            blocksQueue.Peek().AllowDragging(false);
        }
        enabled = false;
    }
}