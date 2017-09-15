// Author(s): Paul Calande, [your name here]

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBlock
{
    int x;

    int y;

    Block block;

    Grid grid;

    public GridBlock(int xStart, int yStart, Block myBlock)
    {
        x = xStart;
        y = xStart;
        block = myBlock;
    }

    public int GetX()
    {
        return x;
    }

    public int GetY()
    {
        return y;
    }

    public void Move(Enums.Direction direction)
    {
        int tempX = x;
        int tempY = y;
        int blockW = block.GetWidth();
        int blockH = block.GetHeight();
        Tile[,] gridTiles = grid.GetTiles();

        switch (direction)
        {
            case Enums.Direction.Down:
                y += blockH;
                break;
            case Enums.Direction.Up:
                y -= blockH;
                break;
            case Enums.Direction.Left:
                x -= blockW;
                break;
            case Enums.Direction.Right:
                x += blockW;
                break;
        }

        if (grid.CanBlockFit(x, y, block))
        {
            Tile[,] blockTiles = block.GetTiles();
            for (int i = 0; i < blockW; i++)
            {
                for (int j = 0; j < blockH; j++)
                {
                    gridTiles[x + i, y + j].Duplicate(blockTiles[i, j]);
                    blockTiles[x + i, y + j].Clear();
                }
            }
        }
        else
        {
            x = tempX;
            y = tempY;
        }
    }
}