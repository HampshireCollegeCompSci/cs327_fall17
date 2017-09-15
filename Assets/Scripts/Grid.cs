// Author(s): Paul Calande, Yifeng Shi
// A 2-dimensional collections of tiles

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    int width;
    int height;

    Tile[,] tiles;
    Space[,] spaces;

    GameObject prefabTile;
    GameObject prefabSpace;

    BlockSpawner blockSpawner;

    List<GridBlock> girdBlocks;

    private void Start()
    {
        //Instantiate tiles array
        tiles = new Tile[width, height];
        for (int i = 0; i < width; i++){
            for (int j = 0; j < height; j++){
                //Need to be changed after knowing specific positions
                GameObject currentPrefabTile = Instantiate(prefabTile);
                tiles[i, j] = currentPrefabTile.GetComponent<Tile>();
            }
        }

        //Instantiate spaces array
        spaces = new Space[width, height];
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				//Need to be changed after knowing specific positions
				GameObject currentPrefabSpace = Instantiate(prefabSpace);
                spaces[i, j] = currentPrefabSpace.GetComponent<Space>();
                //Need to be changed after knowing specific positions.
                spaces[i, j].Init(0, 0, this);
			}
		}

        //Instantiate BlockSpawner
        blockSpawner.Init(spaces);

    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    public Tile[,] GetTiles()
    {
        return tiles;
    }

    public bool CanBlockFit(int x, int y, Block block)
    {
        return true;
    }

    public void WriteBlock(int x, int y, Block block)
    {

    }

    public void CheckForMatches()
    {

    }

    public void MoveAllBlocks(Enums.Direction direction)
    {

    }
}