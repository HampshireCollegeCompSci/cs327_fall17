﻿// Author(s): Paul Calande, [your name here]

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
    int width;

    int height;

    Tile[,] tiles;

    Block(int newWidth, int newHeight)
    {

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

    public void SetTile(int x, int y, Tile newTile)
    {

    }
}