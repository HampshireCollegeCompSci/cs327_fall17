// Author(s): Paul Calande, Yixiang Xu

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

    //Return the x index of the gridblock
    public int GetX()
    {
        return x;
    }

    //Return the y index of the gridblock
    public int GetY()
    {
        return y;
    }

    /*Repeatedly moves the GridBlock along the Grid in
     *the given direction, one Tile at a time, only stopping
     *at occupied Tiles (preventing Tile overlap) or the
     *border.*/
    public bool Move(Enums.Direction direction)
    {
        Tile[,] gridTiles = grid.GetTiles();
        Tile[,] blockTiles = block.GetTiles();
        List<Vector2> exTilesIndex = new List<Vector2>();

        switch (direction)
        {
            case Enums.Direction.Right:
                for (int i = 0; i < block.GetHeight(); i++)
                {
                    int maxX = -1;
                    for (int j = 0; j < block.GetWidth(); j++)
                    {
                        if (blockTiles[j, i].GetIsOccupied())
                        {
                            maxX = j;
                            if (x + maxX + 1 > grid.GetWidth() || (maxX + 1 < block.GetWidth() &&
                            !blockTiles[maxX + 1, i].GetIsOccupied() && gridTiles[x + maxX + 1, i].GetIsOccupied())
                            || (maxX + 1 == block.GetWidth() && gridTiles[x + maxX + 1, i].GetIsOccupied()))
                                return false;
                        }
                    }

                    if (maxX != -1)
                        exTilesIndex.Add(new Vector2(maxX, i));
                }

                for (int i = 0; i < exTilesIndex.Count; i++)
                {
                    int exTileX = (int)exTilesIndex[i].x;
                    int exTileY = (int)exTilesIndex[i].y;

                    for (int j = exTileX; j >= 0; j--)
                    {
                        gridTiles[j + x + 1, y + exTileY].Duplicate(blockTiles[j, exTileY]);
                        gridTiles[j + x, y + exTileY].Clear();
                    }
                }

                x += 1;

                break;
            case Enums.Direction.Down:
                for (int i = 0; i < block.GetWidth(); i++)
                {
                    int maxY = -1;
                    for (int j = 0; j < block.GetHeight(); j++)
                    {
                        if (blockTiles[i, j].GetIsOccupied())
                        {
                            maxY = j;
                            if (y + maxY + 1 > grid.GetHeight() || (maxY + 1 < block.GetHeight() &&
                            !blockTiles[i, maxY + 1].GetIsOccupied() && gridTiles[i, y + maxY + 1].GetIsOccupied())
                            || (maxY + 1 == block.GetHeight() && gridTiles[i, y + maxY + 1].GetIsOccupied()))
                                return false;
                        }
                    }

                    if (maxY != -1)
                        exTilesIndex.Add(new Vector2(i, maxY));
                }

                for (int i = 0; i < exTilesIndex.Count; i++)
                {
                    int exTileX = (int)exTilesIndex[i].x;
                    int exTileY = (int)exTilesIndex[i].y;

                    for (int j = exTileY; j >= 0; j--)
                    {
                        gridTiles[x + exTileX, y + j + 1].Duplicate(blockTiles[exTileX, j]);
                        gridTiles[x + exTileX, y + j].Clear();
                    }
                }

                y += 1;

                break;
            case Enums.Direction.Left:
                for (int i = 0; i < block.GetHeight(); i++)
                {
                    int minX = -1;
                    for (int j = block.GetWidth() - 1; j >= 0; j--)
                    {
                        if (blockTiles[j, i].GetIsOccupied())
                        {
                            minX = j;
                            if (x + minX - 1 < 0 || (minX - 1 >= 0 &&
                            !blockTiles[minX - 1, i].GetIsOccupied() && gridTiles[x + minX - 1, i].GetIsOccupied())
                            || (minX - 1 == -1 && gridTiles[x + minX - 1, i].GetIsOccupied()))
                                return false;
                        }
                    }

                    if (minX != -1)
                        exTilesIndex.Add(new Vector2(minX, i));
                }
                for (int i = 0; i < exTilesIndex.Count; i++)
                {
                    int exTileX = (int)exTilesIndex[i].x;
                    int exTileY = (int)exTilesIndex[i].y;

                    for (int j = 0; j < exTileX; j++)
                    {
                        gridTiles[j + x - 1, y + exTileY].Duplicate(blockTiles[j, exTileY]);
                        gridTiles[j + x, y + exTileY].Clear();
                    }
                }

                x -= 1;

                break;

            case Enums.Direction.Up:
                for (int i = 0; i < block.GetWidth(); i++)
                {
                    int minY = -1;
                    for (int j = block.GetHeight() - 1; j >= 0; j--)
                    {
                        if (blockTiles[i, j].GetIsOccupied())
                        {
                            minY = j;
                            if (y + minY - 1 < 0 || (minY - 1 >= 0 &&
                            !blockTiles[i, minY - 1].GetIsOccupied() && gridTiles[i, y + minY - 1].GetIsOccupied())
                            || (minY - 1 == -1 && gridTiles[i, y + minY - 1].GetIsOccupied()))
                                return false;
                        }
                    }

                    if (minY != -1)
                        exTilesIndex.Add(new Vector2(i, minY));
                }

                for (int i = 0; i < exTilesIndex.Count; i++)
                {
                    int exTileX = (int)exTilesIndex[i].x;
                    int exTileY = (int)exTilesIndex[i].y;

                    for (int j = 0; j < exTileY; j++)
                    {
                        gridTiles[x + exTileX, y + j - 1].Duplicate(blockTiles[exTileX, j]);
                        gridTiles[x + exTileX, y + j].Clear();
                    }
                }

                y -= 1;

                break;
        }

        return true;
    }

    //Runs the Move method with the given direction over and over until Move returns false
    public void MoveRepeatedly(Enums.Direction direction)
    {
        bool isMoved = Move(direction);
        while (isMoved)
        {
            Move(direction);
        }
    }
}
 