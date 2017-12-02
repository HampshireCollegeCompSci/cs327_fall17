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
    // A block that exists within the bags.
    // Contains additional data such as bag information.
    class BagBlock
    {
        Block block;

        // The bags that the block is in.
        List<Bag> bags;

        // Constructor.
        //public BagBlock(Block mblock, List<int> mtiers)
        public BagBlock(Block mblock, List<Bag> mbags)
        {
            block = mblock;
            bags = mbags;
        }

        // Duplicates the contained Block and returns it.
        public Block CreateBlock()
        {
            return new Block(block);
        }

        // Returns true if any of the current bags contain this block.
        public bool AreConditionsSatisfied()
        {
            //Debug.Log("BagBlock.AreConditionsSatisfied: bags.Count: " + bags.Count);
            foreach (Bag bag in bags)
            {
                if (bag.AreConditionsSatisfied())
                {
                    return true;
                }
            }
            return false;
        }

        // Returns true if the BagBlock is a junkyard-only block.
        public bool IsJunkyardOnly()
        {
            foreach (Bag bag in bags)
            {
                if (!bag.IsJunkyard())
                {
                    return false;
                }
            }
            return true;
        }
    }

    // Bags determine the conditions under which blocks are spawned.
    class Bag
    {
        public delegate bool Condition();

        // The list of conditions required to enable the bag.
        List<Condition> preconditions = new List<Condition>();

        // The name of the bag, as determined by JSON.
        string name;

        // Whether or not the bag is a junkyard bag.
        bool isJunkyard = false;

        // The trigger to run when the bag becomes active.
        string trigger = "";

        // Whether or not the block ends the tutorial.
        //bool endsTutorial = false;

        // Constructor.
        public Bag(string mname)
        {
            name = mname;
        }

        public void AddCondition(Condition cond)
        {
            preconditions.Add(cond);
        }

        // Return false if any of the conditions are not met. Otherwise, return true.
        public bool AreConditionsSatisfied()
        {
            foreach (Condition cond in preconditions)
            {
                if (cond() == false)
                {
                    return false;
                }
            }
            if (trigger != "")
            {
                TutorialController.Instance.TriggerEvent(trigger);
            }
            return true;
        }

        public string GetName()
        {
            return name;
        }

        public void LabelAsJunkyard()
        {
            isJunkyard = true;
        }

        public bool IsJunkyard()
        {
            return isJunkyard;
        }

        public void SetTrigger(string newTrigger)
        {
            trigger = newTrigger;
        }

        /*
        public void LabelAsEndsTutorial()
        {
            endsTutorial = true;
        }

        public bool EndsTutorial()
        {
            return endsTutorial;
        }
        */
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
    int tierCurrent = 0;
    [SerializeField]
    [Tooltip("The current number of vestiges to generate per block.")]
    int vestigesPerBlock = 0;
    [SerializeField]
    [Tooltip("The vestige level to use.")]
    int vestigeLevel = 1;
    /*
    [SerializeField]
    [Tooltip("Reference to ScreenTapping.")]
    ScreenTapping screenTapping;
    */
    [SerializeField]
    [Tooltip("Reference to the ScoreCounter.")]
    ScoreCounter scoreCounter;
    [SerializeField]
    [Tooltip("Reference to the TutorialController.")]
    TutorialController tutorialController;

    // List of all possible blocks that can be put into the bag.
    List<BagBlock> possibleBlocks = new List<BagBlock>();
    // List of all different bags.
    List<Bag> allBags = new List<Bag>();
    // List of blocks remaining in the bag.
    List<BagBlock> currentBag = new List<BagBlock>();

    // List of bags that end the tutorial.
    List<Bag> tutorialEnders = new List<Bag>();

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
    //int friendlyBagMaxScore;
    // Whether a junkyard event is currently starting.
    bool junkyardEventIsStarting = false;
    // Whether any blocks in the spawner are allowed to be dragged.
    bool draggingIsAllowed = true;

    private void Awake()
    {
        tierCurrent = 0;
    }

    private void Start()
    {
        gameFlow.GameLost += GameFlow_GameLost;
        //scoreCounter.ScoreChanged += ScoreCounter_ScoreChanged;

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

        JSONArray bagsCategory = json["bags"].AsArray;

        for (int i = 0; i < bagsCategory.Count; ++i)
        {
            JSONNode bagNode = bagsCategory[i];
            string name = bagNode["name"];
            Bag newBag = new Bag(name);

            JSONNode aNode;
            aNode = bagNode["ends tutorial mode"];
            if (aNode != null)
            {
                if (aNode == true)
                {
                    //newBag.LabelAsEndsTutorial();
                    tutorialEnders.Add(newBag);
                }
            }

            aNode = bagNode["trigger when starting"];
            if (aNode != null)
            {
                newBag.SetTrigger(aNode);
            }

            JSONNode condsNode = bagNode["preconditions"];
            JSONNode cond;

            cond = condsNode["tutorial is active"];
            if (cond != null)
            {
                bool compTo = cond.AsBool;
                newBag.AddCondition(() =>
                {
                    /*
                    Debug.Log(newBag.GetName() + ": compTo / tutenabled: "
                        + compTo + " / " + Settings.Instance.IsTutorialModeEnabled());
                        */
                    return (compTo == Settings.Instance.IsTutorialModeEnabled());
                });
            }
            cond = condsNode["square clearings count is equal to"];
            if (cond != null)
            {
                int compTo = cond.AsInt;
                newBag.AddCondition(() => compTo == grid.GetSquareClearingsCount());
            }
            cond = condsNode["score is equal to"];
            if (cond != null)
            {
                int compTo = cond.AsInt;
                newBag.AddCondition(() => compTo == scoreCounter.GetScore());
            }
            cond = condsNode["score is not equal to"];
            if (cond != null)
            {
                int compTo = cond.AsInt;
                newBag.AddCondition(() => compTo != scoreCounter.GetScore());
            }
            cond = condsNode["score is greater than"];
            if (cond != null)
            {
                int compTo = cond.AsInt;
                newBag.AddCondition(() =>
                {
                    //Debug.Log(newBag.GetName() + ": compTo / score: " + compTo + " / " + scoreCounter.GetScore());
                    return (compTo < scoreCounter.GetScore());
                });
            }
            cond = condsNode["score is less than"];
            if (cond != null)
            {
                int compTo = cond.AsInt;
                newBag.AddCondition(() => compTo > scoreCounter.GetScore());
            }
            cond = condsNode["junkyard tier is equal to"];
            if (cond != null)
            {
                int compTo = cond.AsInt;
                newBag.AddCondition(() =>
                {
                    //Debug.Log(newBag.GetName() + ": compTo / tierCurrent: " + compTo + " / " + tierCurrent);
                    return (compTo == tierCurrent);
                });
                if (compTo != 0)
                {
                    newBag.LabelAsJunkyard();
                }
            }

            allBags.Add(newBag);
        }

        JSONArray blocksCategory = json["blocks"].AsArray;

        for (int i = 0; i < blocksCategory.Count; ++i)
        {
            JSONNode blockNode = blocksCategory[i];

            int w = blockNode["width"].AsInt;
            int h = blockNode["height"].AsInt;
            int sprite = blockNode["sprite"].AsInt;
            JSONArray cell = blockNode["cells"].AsArray;
            JSONArray bagsArray = blockNode["bags"].AsArray;

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

            // List of all the bags that this new block is a part of.
            List<Bag> bagList = new List<Bag>();
            int numBags = bagsArray.Count;
            for (int t = 0; t < numBags; ++t)
            {
                string bagName = bagsArray[t];
                Bag theBag = allBags.Find(x => x.GetName() == bagName);
                bagList.Add(theBag);
            }
            //possibleBlocks.Add(new BagBlock(block, tierList));
            BagBlock newBagBlock = new BagBlock(block, bagList);
            possibleBlocks.Add(newBagBlock);
        }

        json = JSON.Parse(tuningJSON.ToString());
        doContaminationBlocksAlternate = json["contamination blocks alternate"].AsBool;
        //friendlyBagMaxScore = json["friendly bag max score"].AsInt;
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

    // Get all blocks whose bags satisfy the current conditions of the game.
    private List<BagBlock> GetSatisfiedBagBlocks()
    {
        return possibleBlocks.FindAll(x => x.AreConditionsSatisfied());
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

                // Check if the tutorial has ended yet.
                List<Bag> tutEndBags = tutorialEnders.FindAll(x => x.AreConditionsSatisfied());
                if (tutEndBags.Count != 0)
                {
                    tutorialController.EndTutorialMode();
                    enabled = false;
                    return;
                }

                // Check if conditions have changed.
                List<BagBlock> diff = currentBag.FindAll(x => x.AreConditionsSatisfied());
                if (diff.Count != currentBag.Count)
                {
                    // If conditions have changed, clear the bag so that it gets replaced.
                    currentBag.Clear();
                }

                if (currentBag.Count == 0)
                {
                    //currentBag = possibleBlocks.FindAll(x => x.AreConditionsSatisfied());
                    currentBag = GetSatisfiedBagBlocks();
                    //Debug.Log("BlockSpawner.SpawnRandomBlock: currentBag size: " + currentBag.Count);
                    //Debug.Log("Score: " + scoreCounter.GetScore());

                    // If a Junkyard event is starting, choose a junkyard-specific block.
                    if (junkyardEventIsStarting)
                    {
                        junkyardEventIsStarting = false;

                        //List<BagBlock> junkyardBlocks = GetBagBlocksJunkyardOnlyOfTier(tierCurrent);
                        List<BagBlock> junkyardBlocks = currentBag.FindAll(x => x.IsJunkyardOnly());

                        int indexInJunkyardBlocks = Random.Range(0, junkyardBlocks.Count);
                        BagBlock junkyardBlock = junkyardBlocks[indexInJunkyardBlocks];

                        for (int j = 0; j < currentBag.Count; ++j)
                        {
                            BagBlock bagBlock = currentBag[j];
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
                        indexInBagToChoose = Random.Range(0, currentBag.Count);
                    }
                }
                else
                {
                    indexInBagToChoose = Random.Range(0, currentBag.Count);
                }
                toSpawn = currentBag[indexInBagToChoose].CreateBlock();
                // Remove each chosen element from the bag.
                currentBag.RemoveAt(indexInBagToChoose);

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

            //newDraggable.SetScreenTapping(screenTapping);//Pass screenTapping to DraggableObject

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

            if (!draggingIsAllowed)
            {
                newDraggable.AllowDragging(false);
            }

            // Insert the block into the console grid.
            consoleGrid.SetDraggableBlock(newDraggable);
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
        currentBag.Clear();
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

    public List<Block> GetListOfPossibleBlocksInBag()
    {
        List<BagBlock> bagBlocks = GetSatisfiedBagBlocks();
        List<Block> result = new List<Block>();
        foreach(BagBlock bagBlock in bagBlocks)
        {
            result.Add(bagBlock.CreateBlock());
        }
        return result;
    }

    public void DisableDragging()
    {
        if (blocksQueue.Count > 0)
        {
            blocksQueue.Peek().AllowDragging(false);
        }
        draggingIsAllowed = false;
    }

    // Callback function for gameFlow's GameLost event.
    private void GameFlow_GameLost(GameFlow.GameOverCause cause)
    {
        DisableDragging();
        if (cause == GameFlow.GameOverCause.NoMoreEnergy)
        {
            enabled = false;
        }
    }

    /*
    private void ScoreCounter_ScoreChanged(int newScore)
    {
        // If the score is high enough, abandon the friendly bag.
        if (tierCurrent == -1 && newScore > friendlyBagMaxScore)
        {
            SetJunkyardTier(0);
        }
    }
    */
}