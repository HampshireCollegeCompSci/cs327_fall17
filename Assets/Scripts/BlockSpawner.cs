// Author(s): Paul Calande, Yifeng Shi, Yixiang Xu
/* This class is working for spawning the random blocks
 * and maintaining the queue of incoming blocks.
 */
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SimpleJSON;

public class BlockSpawner : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The number of seconds between DraggableBlock spawns in the queue. Use -1 to disable this feature.")]
    float timeBetweenBlocks;
    [SerializeField]
    [Tooltip("The maximum number of DraggableBlocks in the queue.")]
    int maxBlocksInQueue;
    [SerializeField]
    [Tooltip("The GameObjects determining the position of each queue entry.")]
    GameObject[] blockPositions;
    [SerializeField]
    [Tooltip("Reference to the GameFlow instance.")]
    GameFlow gameFlow;
    [SerializeField]
    [Tooltip("Reference to the Grid instance.")]
    Grid grid;
    [SerializeField]
    [Tooltip("The prefab to instantiate for DraggableBlocks.")]
    GameObject prefabDraggableBlock;
    [SerializeField]
    [Tooltip("Reference to the PossibleBlocks JSON, which determines the block designs.")]
    TextAsset possibleBlocksJSON;
    [SerializeField]
    [Tooltip("The current junkyard tier. 0 = no junkyard event.")]
    int tierCurrent = 0;
    [SerializeField]
    [Tooltip("The current number of vestiges to generate per block.")]
    int vestigesPerBlock = 0;
    [SerializeField]
    [Tooltip("The vestige level to use.")]
    int vestigeLevel = 1;
    [SerializeField]
    [Tooltip("Reference to ScreenTapping.")]
    ScreenTapping screenTapping;

    List<Block>[] possibleBlocks = new List<Block>[4];
    List<Block> bag = new List<Block>();

    Queue<DraggableBlock> blocksQueue = new Queue<DraggableBlock>();

    float timeBeforeNextBlock;

    private void Awake()
    {
        for (int i = 0; i < 4; ++i)
        {
            possibleBlocks[i] = new List<Block>();
        }
    }

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
        //read json file
        var json = JSON.Parse(possibleBlocksJSON.ToString());

        const string blocksNormal = "blocks";

        for (int i = 0; i < json[blocksNormal].Count; i++)
        {
            Block block;
            //int formation = Random.Range(0, json["blocks"].Count);
            var w = json[blocksNormal][i]["width"].AsInt;
            var h = json[blocksNormal][i]["height"].AsInt;
            int sprite = json[blocksNormal][i]["sprite"].AsInt;
            var cell = json[blocksNormal][i]["cells"].AsArray;
            var tiers = json[blocksNormal][i]["tiers"].AsArray;
            //Debug.Log(cell.ToString());

            block = new Block(h, w);

            for (int row = 0; row < h; ++row)
            {
                for (int col = 0; col < w; ++col)
                {
                    int currentPos = col + w * row;
                    int thisCell = cell[currentPos];
                    if (thisCell == 0)
                    {
                        block.Fill(row, col, TileData.TileType.Unoccupied);
                    }
                    else if (thisCell == 1)
                    {
                        block.Fill(row, col, TileData.TileType.Regular);
                    }
                    block.SetSpriteIndex(row, col, sprite);
                }
            }

            int numTiers = tiers.Count;
            for (int t = 0; t < numTiers; ++t)
            {
                int tierID = tiers[t];
                possibleBlocks[tierID].Add(block);
            }
        }

        ResetBlockTimer();
        SpawnRandomBlock();
    }

    private void Update()
    {
        // if the timeBetweenBlocks is not -1 timer will function
        if (timeBetweenBlocks != -1){
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
        if (blocksQueue.Count == maxBlocksInQueue)
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

            if (bag.Count == 0)
            {
                // If the bag is empty, repopulate it.
                for (int j = 0; j < possibleBlocks[tierCurrent].Count; ++j)
                {
                    bag.Add(new Block(possibleBlocks[tierCurrent][j]));
                }
            }

			int i = Random.Range(0, bag.Count);
            Block toSpawn = bag[i];
            // Remove each chosen element from the bag.
            bag.RemoveAt(i);

            // Add vestiges to the block, if applicable.
            int vestigesAdded = 0;
            List<TileData> refs = toSpawn.GetReferencesToType(TileData.TileType.Regular);

            // Stop adding vestiges when there are no regular tiles left.
            //Debug.Log("Vestige generation begin.");
            while (vestigesAdded < vestigesPerBlock && refs.Count != 0)
            {
                int index = Random.Range(0, refs.Count);
                TileData v = refs[index];
                v.Fill(TileData.TileType.Vestige);
                v.SetVestigeLevel(vestigeLevel);
                refs.RemoveAt(index);
                ++vestigesAdded;
                //Debug.Log("vestigesAdded / vestigesPerBlock: " + vestigesAdded + " / " + vestigesPerBlock);
                //Debug.Log("BlockSpawner: vestigeLevel: " + vestigeLevel);
            }
            //Debug.Log("Vestige generation end.");

            // Instantiate the actual block.
            GameObject newBlock = Instantiate(prefabDraggableBlock, transform, false);
            // Initialize the DraggableBlock component.
            DraggableBlock newDraggable = newBlock.GetComponent<DraggableBlock>();
            //newDraggable.Init(toSpawn, grid, canvas);
            newDraggable.Init(toSpawn, grid, GetComponent<RectTransform>());

            newDraggable.SetScreenTapping(screenTapping);//Pass screenTapping to DraggableObject

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
        if (blocksQueue.Count > 0)
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
            AudioController.Instance.RotateTile();
        }
    }

    public void FlipCurrentBlock()
    {
        if (blocksQueue.Count > 0)
        {
            blocksQueue.Peek().Flip();
            AudioController.Instance.RotateTile();
        }
    }

    public void SetJunkyardTier(int newTier)
    {
        tierCurrent = newTier;
        // Clear the bag to force a new bag to generate.
        bag.Clear();
    }

    public void SetVestigesPerBlock(int newVal)
    {
        vestigesPerBlock = newVal;
    }

    public void SetVestigeLevel(int newVal)
    {
        vestigeLevel = newVal;
    }

    public void ForceUpdateSpaceInformation()
    {
        EnableFrontBlock();
        UpdateAllBlocks();
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