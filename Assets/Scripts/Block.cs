// Author(s): Paul Calande, Yixiang Xu(Eric)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
    [SerializeField]
    int width;
    [SerializeField]
    int height;

    TileData[,] tiles;

    public Block(int newWidth, int newHeight)
    {
        width = newWidth;
        height = newHeight;
        tiles = new TileData[width, height];
    }

    //Return the width of the block
    public int GetWidth()
    {
        return width;
    }

    //Return the height of the block
    public int GetHeight()
    {
        return height;
    }

    /*
    //Return the collections of Tiles of the block
    public TileData[,] GetTiles()
    {
        return tiles;
    }
    */

    public void Fill(int row, int col)
    {
        tiles[row, col].Fill();
    }

    public void Clear(int row, int col)
    {
        tiles[row, col].Clear();
    }

    public TileData.TileType GetTileType(int row, int col)
    {
        return tiles[row, col].GetTileType();
    }

    public void SetTileType(int row, int col, TileData.TileType type)
    {
        tiles[row, col].SetTileType(type);
    }

    public bool GetIsOccupied(int row, int col)
    {
        return tiles[row, col].GetIsOccupied();
    }

    public Tile.ChangedHandler GetCallbackTileDataSetTileType(int row, int col)
    {
        return tiles[row, col].SetTileType;
    }

    public void Rotate(bool clockwise)
    {
        int newWidth = height;
        int newHeight = width;
        width = newWidth;
        height = newHeight;
        //TileData[,] tempTiles = new TileData[width, height];

		/*
		for (int column = 0; column < width; ++column) {
			for (int row = 0; row < height; ++row) {
				TileData.TileType newType = tiles[].GetTileType();
				tempTiles [row, column].SetTileType (newType);
			}
		}
		*/

		// Rotation code from:
		// https://www.codeproject.com/Questions/854268/Rotate-a-matrix-degrees-cloclwise
		for (int i = 0; i < height; ++i) {
			for (int j = i + 1; j < width; ++j) {
				TileData temp = tiles [i, j];
				tiles [i, j] = tiles [j, i];
				tiles [j, i] = temp;
			}
		}
		for (int i = 0; i < height; ++i) {
			for (int j = 0; j < width / 2; ++j) {
				TileData temp = tiles [i, j];
				tiles [i, j] = tiles [i, width - 1 - j];
				tiles [i, width - 1 - j] = temp;
			}
		}


		/*
        if (clockwise == true)
        {
            //rotates blocks to the right
            tempTiles[0, 0].SetTileType(tiles[1, 0].GetTileType());
            tempTiles[0, 1].SetTileType(tiles[0, 0].GetTileType());
            tempTiles[1, 1].SetTileType(tiles[0, 1].GetTileType());
            tempTiles[1, 0].SetTileType(tiles[1, 1].GetTileType());
        }
        else
        {
            //rotates blocks to the left
            tempTiles[0, 0].SetTileType(tiles[0, 1].GetTileType());
            tempTiles[0, 1].SetTileType(tiles[1, 1].GetTileType());
            tempTiles[1, 1].SetTileType(tiles[1, 0].GetTileType());
            tempTiles[1, 0].SetTileType(tiles[0, 0].GetTileType());
        }
        */

        //tiles = tempTiles;
    }
}