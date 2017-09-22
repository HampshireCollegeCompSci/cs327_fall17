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

    public void Fill(int row, int col, TileData.TileType newType)
    {
        tiles[row, col].Fill(newType);
    }

    public void Clear(int row, int col)
    {
        tiles[row, col].Clear();
    }

    public TileData.TileType GetTileType(int row, int col)
    {
        return tiles[row, col].GetTileType();
    }

    public bool GetIsOccupied(int row, int col)
    {
        return tiles[row, col].GetIsOccupied();
    }

    public Tile.ChangedHandler GetCallbackTileDataSetTileType(int row, int col)
    {
        return tiles[row, col].Fill;
    }

    // Rotates the Block.
    // TODO: Support counterclockwise rotation.
    public void Rotate(bool clockwise)
    {
        // Height and width are swapped when rotating 90 degrees.
        TileData[,] tilesTemp = new TileData[height, width];

        // Rotation algorithm adapted from:
        // https://www.codeproject.com/Questions/854268/Rotate-a-matrix-degrees-cloclwise

        // Transpose.
        for (int i = 0; i < height; ++i)
        {
            for (int j = i + 1; j < width; ++j)
            {
                tilesTemp[i, j] = tiles[j, i];
                tilesTemp[j, i] = tiles[i, j];
            }
        }
        if (clockwise)
        {
            // Reverse each row.
            for (int i = 0; i < height; ++i)
            {
                for (int j = 0; j < width / 2; ++j)
                {
                    tilesTemp[i, j] = tiles[i, width - 1 - j];
                    tilesTemp[i, width - 1 - j] = tiles[i, j];
                }
            }
        }
        else
        {
            // Reverse each column.
            for (int i = 0; i < height; ++i)
            {
                for (int j = 0; j < width / 2; ++j)
                {
                    tilesTemp[i, j] = tiles[height - 1 - i, j];
                    tilesTemp[height - 1 - i, j] = tiles[i, j];
                }
            }
        }

        // Update the tiles array with the new Tiles.
        tiles = tilesTemp;
        // Swap width and height.
        int temp = width;
        width = height;
        height = temp;

        /*
        if (!clockwise)
        {
            // Rotate clockwise two additional times to create a counterclockwise rotation.
            Rotate(true);
            Rotate(true);
        }
        */
    }
}