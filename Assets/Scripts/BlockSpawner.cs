// Author(s): Paul Calande, Yifeng Shi
/* This class is working for spawniong the random blocks
 * and maintaining the queue of incoming blocks
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
    //File blockData;

    List<Block> possibleBlocks = new List<Block>();

    Queue<DraggableBlock> blocksQueue = new Queue<DraggableBlock>();

    float timeBeforeNextBlock;


    private void Start()
    {
        //leave for now about the File reading
    }

    /*
    public void Init(Grid newGrid)
    {
        grid = newGrid;
    }
    */

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
		if (maxBlocksInQueue > 1) {
			if (blocksQueue.Count == maxBlocksInQueue) {
				//if the # of elements in queue already reaches
				//max, game over
				gameFlow.GameOver ();
				return;
			} else {
				//otherwise we select a random block from the possible list,
				//then instantiate the draggable block and add it into the queue.
				int i = Random.Range (0, possibleBlocks.Count);
				Block toSpawn = possibleBlocks [i];
				prefabDraggableBlock.GetComponent<DraggableBlock> ().SetBlock (toSpawn);
				Instantiate (prefabDraggableBlock);          
				blocksQueue.Enqueue (prefabDraggableBlock.GetComponent<DraggableBlock> ());
				//if this block is the only block in queue, enable it
				if (blocksQueue.Count == 1)
					EnableFrontBlock ();
				PositionBlocks ();
			} 
		}
        else
        {
            //otherwise we select a random block from the possible list,
            //then instantiate the draggable block and add it into the queue.
            int i = Random.Range(0, possibleBlocks.Count);
            Block toSpawn = possibleBlocks[i];
            prefabDraggableBlock.GetComponent<DraggableBlock>().SetBlock(toSpawn);
            Instantiate(prefabDraggableBlock);          
            blocksQueue.Enqueue(prefabDraggableBlock.GetComponent<DraggableBlock>());
            //if this block is the only block in queue, enable it
            if (blocksQueue.Count == 1)
                EnableFrontBlock();
            PositionBlocks();
        }
    }

    public void ProgressQueue()
    {
        //Dequeue the blocks at the front
        blocksQueue.Dequeue();
        
        if(blocksQueue.Count == 0)
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
        PositionBlocks();
    }

    void EnableFrontBlock()
    {
        //Check the size of queue just for safety
        if(blocksQueue.Count > 0)
        {
            //Setup the front block ready
            blocksQueue.Peek().AllowDragging();
            //If there is no more room for block, game over
            if (grid.CheckIfSpacesFilled(blocksQueue.Peek().GetBlock()))
                gameFlow.GameOver();
        }
    }

    void PositionBlocks()
    {
        //Set each of the blocks in the queue
        //to its corresponding position
        int i = 0;
        foreach(DraggableBlock db in blocksQueue)
        {
            db.transform.position = blockPositions[i].transform.position;
            i++;
        }
    }

    public void RotateCurrentBlock(bool clockwise)
    {
        if(blocksQueue.Count > 0)
            blocksQueue.Peek().Rotate(clockwise);
    }
}