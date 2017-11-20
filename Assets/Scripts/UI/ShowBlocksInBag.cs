using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowBlocksInBag : MonoBehaviour {

    [SerializeField]
    [Tooltip("Reference to the block spawner object.")]
    BlockSpawner blockSpawner;
    [SerializeField]
    [Tooltip("The BlockInBag prefab to instantiate.")]
    GameObject prefabBlockInBag;
    [SerializeField]
    Grid grid;

    List<GameObject> blocksInBag;

    void Awake()
    {
        Debug.Log("A");
        blocksInBag = new List<GameObject>();
    }

    public void ShowBlocks()
    {
        if (blocksInBag.Count > 0)
        {
            foreach(GameObject block in blocksInBag)
                Destroy(block);

            blocksInBag.Clear();
        }

        List<Block> blocks = blockSpawner.GetListOfPossibleBlocksInBag();
        float cellX = GetComponent<GridLayoutGroup>().cellSize.x;
        float cellY = GetComponent<GridLayoutGroup>().cellSize.y;

        foreach (Block block in blocks)
        {
            GameObject blockInBag = Instantiate(prefabBlockInBag, transform);
            blockInBag.GetComponent<BlockInBag>().Init(block, cellX, cellY, grid.GetTileHeight(), grid.GetTileWidth());
            blocksInBag.Add(blockInBag);
        }
    }
}
