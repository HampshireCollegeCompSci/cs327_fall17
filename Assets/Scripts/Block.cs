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

    public Block(int newHeight, int newWidth)
    {
        width = newWidth;
        height = newHeight;
        // Initialize rows first, columns second.
        tiles = new TileData[height, width];
        // Construct TileDatas.
        for (int row = 0; row < height; ++row)
        {
            for (int col = 0; col < width; ++col)
            {
                tiles[row, col] = new TileData();
            }
        }
    }

    // Copy constructor for Block.
    public Block(Block other)
    {
        width = other.width;
        height = other.height;
        tiles = new TileData[height, width];
        for (int row = 0; row < height; ++row)
        {
            for (int col = 0; col < width; ++col)
            {
                tiles[row, col] = new TileData(other.tiles[row, col]);
            }
        }
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

    public Tile.ChangedHandler GetCallbackTileDataFill(int row, int col)
    {
        return tiles[row, col].Fill;
    }

    // Rotates the Block.
    public void Rotate(bool clockwise)
    {
        //Debug.Log("Pre-rotation: " + ToString());

        TileData[,] newTiles = new TileData[width, height];

        if (clockwise)
        {
            for (int row = 0; row < height; ++row)
            {
                for (int col = 0; col < width; ++col)
                {
                    newTiles[width - 1 - col, row] = new TileData(tiles[row, col]);
                }
            }
        }
        else
        {
            for (int row = 0; row < height; ++row)
            {
                for (int col = 0; col < width; ++col)
                {
                    newTiles[col, height - 1 - row] = new TileData(tiles[row, col]);
                }
            }
        }

        // Update the tiles array with the new Tiles.
        tiles = newTiles;

        // Swap width and height.
        int temp = width;
        width = height;
        height = temp;

        //Debug.Log("Post-rotation: " + ToString());
    }

    // Converts Block information to a string.
    public override string ToString()
    {
        string ret = height + " x " + width + " Block: ";
        for (int row = 0; row < height; ++row)
        {
            for (int col = 0; col < width; ++col)
            {
                ret += "(" + row + ", " + col + "): " + GetTileType(row, col) + ", ";
            }
        }
        return ret;
    }

    // Returns true if the entire Block is made of this type of tile.
    public bool GetIsEntirely(TileData.TileType type)
    {
        foreach (TileData tile in tiles)
        {
            if (tile.GetTileType() != type)
            {
                return false;
            }
        }
        return true;
    }
}