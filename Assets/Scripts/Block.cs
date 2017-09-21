// Author(s): Paul Calande, Yixiang Xu(Eric)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
    int width;

    int height;

    TileData[,] tiles;

    public Block(int newWidth, int newHeight)
    {
        tiles = new TileData[newWidth, newHeight];
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

    public void Fill(int x, int y)
    {
        tiles[x, y].Fill();
    }

    public void Clear(int x, int y)
    {
        tiles[x, y].Clear();
    }

    public TileData.TileType GetTileType(int x, int y)
    {
        return tiles[x, y].GetTileType();
    }

    public void SetTileType(int x, int y, TileData.TileType type)
    {
        tiles[x, y].SetTileType(type);
    }

    public bool GetIsOccupied(int x, int y)
    {
        return tiles[x, y].GetIsOccupied();
    }

    public Tile.ChangedHandler GetCallbackTileDataSetTileType(int x, int y)
    {
        return tiles[x, y].SetTileType;
    }

    public void Rotate(bool clockwise)
    {

    }
}