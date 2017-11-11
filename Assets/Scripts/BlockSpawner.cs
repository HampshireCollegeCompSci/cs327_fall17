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
    // A block that exists within the bag.
    // Contains additional data such as tier information.
    class BagBlock
    {
        Block block;
        List<int> tiers;

        // Constructor.
        public BagBlock(Block mblock, List<int> mtiers)
        {
            block = mblock;
            tiers = mtiers;
        }

        // Returns true if the BagBlock is in the given tier.
        public bool IsTier(int tier)
        {
            return tiers.Contains(tier);
        }

        // Returns true if the BagBlock is a junkyard-only block.
        public bool IsJunkyardOnly()
        {
            int smallestTier = tiers[0];
            for (int i = 1; i < tiers.Count; ++i)
            {
                int tier = tiers[i];
                if (tier < smallestTier)
                {
                    smallestTier = tier;
                }
            }
            return smallestTier > 0;
        }

        // Duplicates the contained Block and returns it.
        public Block CreateBlock()
        {
            return new Block(block);
        }
    }

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
    [Tooltip("Reference to the Grid instance.")]
    ConsoleGrid consoleGrid;
    [SerializeField]
    [Tooltip("The prefab to instantiate for DraggableBlocks.")]
    GameObject prefabDraggableBlock;
    [SerializeField]
    [Tooltip("Reference to the PossibleBlocks JSON, which determines the block designs.")]
    TextAsset possibleBlocksJSON;
    [SerializeField]
    [Tooltip("Reference to the Tuning JSON.")]
    TextAsset tuningJSON;
    [SerializeField]
    [Tooltip("The current junkyard tier. 0 = no junkyard event. -1 = friendly bag only.")]
    int tierCurrent = -1;
    [SerializeField]
    [Tooltip("The current number of vestiges to generate per block.")]
    int vestigesPerBlock = 0;
    [SerializeField]
    [Tooltip("The vestige level to use.")]
    int vestigeLevel = 1;
    [SerializeField]
    [Tooltip("Reference to ScreenTapping.")]
    ScreenTapping screenTapping;
    [SerializeField]
    [Tooltip("Reference to the ScoreCounter.")]
    ScoreCounter scoreCounter;

    // List of all possible blocks that can be put into the bag.
    List<BagBlock> possibleBlocks = new List<BagBlock>();
    // List of blocks remaining in the bag.
    List<BagBlock> bag = new List<BagBlock>();

    Queue<DraggableBlock> blocksQueue = new Queue<DraggableBlock>();

    float timeBeforeNextBlock;
    Block currentBlock;
    bool isSpawningSameBlocks;
    bool isSpawningOneTileBlocks;
    // Keeps track of whether the block shall contain vestiges or not.
    bool isContaminationBlock = true;
    // Whether contamination blocks alternate or not.
    bool doContaminationBlocksAlternate;
    // The maximum score for the friendly bag. When the score becomes larger than this value,
    // the friendly bag gets abandoned.
    int friendlyBagMaxScore;
    // Whether a junkyard event is currently starting.
    bool junkyardEventIsStarting = false;

    private void Awake()
    {
        tierCurrent = -1;
        /*
        foreach (int tier in tiers)
        {
            possibleBlocks.Add(tier, new List<Block>());
        }
        */
    }

    private void Start()
    {
        gameFlow.GameLost += GameFlow_GameLost;
        scoreCounter.ScoreChanged += ScoreCounter_ScoreChanged;

        // Make sure that we can't have more DraggableBlocks in the queue
        // than there are block positions!
        if (maxBlocksInQueue > blockPositions.Length)
        {
            maxBlocksInQueue = blockPositions.Length;
        }
    }

    private void Tune()
    {
        // Read PossibleBlocks JSON.
        JSONNode json = JSON.Parse(possibleBlocksJSON.ToString());

        const string blocksNormal = "blocks";

        for (int i = 0; i < json[blocksNormal].Count; i++)
        {
            JSONNode blockNode = json[blocksNormal][i];

            int w = blockNode["width"].AsInt;
            int h = blockNode["height"].AsInt;
            int sprite = blockNode["sprite"].AsInt;
            JSONArray cell = blockNode["cells"].AsArray;
            JSONArray tiers = blockNode["tiers"].AsArray;

            Block block = new Block(h, w);

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

            List<int> tierList = new List<int>();
            int numTiers = tiers.Count;
            for (int t = 0; t < numTiers; ++t)
            {
                int tierID = tiers[t];
                tierList.Add(tierID);
            }
            possibleBlocks.Add(new BagBlock(block, tierList));
        }

        json = JSON.Parse(tuningJSON.ToString());
        doContaminationBlocksAlternate = json["contamination blocks alternate"].AsBool;
        friendlyBagMaxScore = json["friendly bag max score"].AsInt;
    }

    public void Init()
    {
        Tune();
        ResetBlockTimer();
        SpawnRandomBlock();
    }

    private void Update()
    {
        // if the timeBetweenBlocks is not -1 timer will function
        if (timeBetweenBlocks != -1)
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
    }

    private List<BagBlock> GetBagBlocksOfTier(int tier)
    {
        return possibleBlocks.FindAll((BagBlock x) => x.IsTier(tier));
    }

    private List<BagBlock> GetBagBlocksJunkyardOnly()
    {
        return possibleBlocks.FindAll((BagBlock x) => x.IsJunkyardOnly());
    }

    private List<BagBlock> GetBagBlocksJunkyardOnlyOfTier(int tier)
    {
        return possibleBlocks.FindAll(x => x.IsTier(tier) && x.IsJunkyardOnly());
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
            Block toSpawn = null;

            if (isSpawningOneTileBlocks)
            {
                Block oneTile = new Block(1, 1);
                oneTile.Fill(0, 0, TileData.TileType.Regular);
                toSpawn = oneTile;
            }
            else if (isSpawningSameBlocks)
            {
                if (currentBlock != null)
                {
                    toSpawn = currentBlock;
                }
            }
            else
            {
                // The index of the block to choose from the bag.
                int indexInBagToChoose = -1;
                if (bag.Count == 0)
                {
                    // If the bag is empty, repopulate it.
                    List<BagBlock> viableBlocks = GetBagBlocksOfTier(tierCurrent);
                    for (int j = 0; j < viableBlocks.Count; ++j)
                    {
                        //Block bagBlock = possibleBlocks[tierCurrent][j];
                        //Block bagBlock = new Block(possibleBlocks[tierCurrent][j]);
                        BagBlock viableBlock = viableBlocks[j];
                        bag.Add(viableBlock);
                    }
                    // If a Junkyard event is starting, choose a junkyard-specific block.
                    if (junkyardEventIsStarting)
                    {
                        junkyardEventIsStarting = false;

                        List<BagBlock> junkyardBlocks = GetBagBlocksJunkyardOnlyOfTier(tierCurrent);

                        int indexInJunkyardBlocks = Random.Range(0, junkyardBlocks.Count);
                        BagBlock junkyardBlock = junkyardBlocks[indexInJunkyardBlocks];

                        for (int j = 0; j < bag.Count; ++j)
                        {
                            BagBlock bagBlock = bag[j];
                            if (bagBlock == junkyardBlock)
                            {
                                indexInBagToChoose = j;
                                break;
                            }
                        }
                        /*
                        Debug.Log("Junkyard index / Bag index: " + indexInJunkyardBlocks +
                            " / " + indexInBagToChoose);
                            */
                    }
                    else
                    {
                        indexInBagToChoose = Random.Range(0, bag.Count);
                    }
                }
                else
                {
                    indexInBagToChoose = Random.Range(0, bag.Count);
                }
                toSpawn = bag[indexInBagToChoose].CreateBlock();
                // Remove each chosen element from the bag.
                bag.RemoveAt(indexInBagToChoose);

                // Add vestiges to the block, if applicable.
                if (isContaminationBlock)
                {
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
                }
                //Debug.Log("Vestige generation end.");
                if (doContaminationBlocksAlternate)
                {
                    isContaminationBlock = !isContaminationBlock;
                }
            }

            // Instantiate the actual block.
            GameObject newBlock = Instantiate(prefabDraggableBlock, transform, false);
            // Initialize the DraggableBlock component.
            DraggableBlock newDraggable = newBlock.GetComponent<DraggableBlock>();
            //newDraggable.Init(toSpawn, grid, canvas);
            newDraggable.Init(toSpawn, grid, GetComponent<RectTransform>(), consoleGrid);

            newDraggable.SetScreenTapping(screenTapping);//Pass screenTapping to DraggableObject

            currentBlock = new Block(newDraggable.GetBlock());

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

            consoleGrid.SetDraggableBlock(newDraggable); //Insert the block into the console grid
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
        if (blocksQueue.Count > 0)
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
            AudioController.Instance.FlipTile();
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
        isContaminationBlock = true;
    }

    public void SetVestigeLevel(int newVal)
    {
        vestigeLevel = newVal;
    }

    public void ForceUpdateSpaceInformation()
    {
        UpdateAllBlocks();
        EnableFrontBlock();
    }

    public void BeginJunkyardEvent()
    {
        junkyardEventIsStarting = true;
    }

    public void EndJunkyardEvent()
    {
        junkyardEventIsStarting = false;
    }

    //Will always spwan the same blocks as the current one
    public void CheatSpawning(bool on)
    {
        isSpawningSameBlocks = on;
    }

    public void OneTileCheatSpawning(bool on)
    {
        isSpawningOneTileBlocks = on;
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

    private void ScoreCounter_ScoreChanged(int newScore)
    {
        // If the score is high enough, abandon the friendly bag.
        if (tierCurrent == -1 && newScore > friendlyBagMaxScore)
        {
            SetJunkyardTier(0);
        }
    }
}