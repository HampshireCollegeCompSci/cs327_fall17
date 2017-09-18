// Author(s): Paul Calande, Yixiang Xu(Eric)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
    int width;

    int height;

    Tile[,] tiles;

    public Block(int newWidth, int newHeight)
    {
        tiles = new Tile[newWidth, newHeight];
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

    //Return the collections of Tiles of the block
    public Tile[,] GetTiles()
    {
        return tiles;
    }

    //Set the Tile of given indexes x and y to new Tile
    public void SetTile(int x, int y, Tile newTile)
    {
        tiles[x, y] = newTile;
    }

    public void Rotate()
    {

    }
}