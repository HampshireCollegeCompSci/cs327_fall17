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

        //The order to run through each tile of the block will be different according to direction
        switch (direction)
        {
            case Enums.Direction.Right:
                //Run through the tiles from left to right, top to bottom, and add right-most extremetiles of each row to the list
                for (int i = 0; i < block.GetHeight(); i++)
                {
                    int maxX = -1; //Record the x index for right-most extremtile
                    for (int j = 0; j < block.GetWidth(); j++)
                    {
                        if (blockTiles[i, j].GetIsOccupied())
                        {
                            maxX = j;

                            //Check conditions which stop the block from moving. Return false if there is one condition fulfilled
                            //Three conditions: block on the edge, obstruction inside the block, obstruction outside the block
                            if (x + maxX + 1 > grid.GetWidth() || (maxX + 1 < block.GetWidth() &&
                            !blockTiles[i, maxX + 1].GetIsOccupied() && gridTiles[i, x + maxX + 1].GetIsOccupied())
                            || (maxX + 1 == block.GetWidth() && gridTiles[i, x + maxX + 1].GetIsOccupied()))
                                return false;
                        }
                    }

                    //If max equals to -1, then the row is empty
                    if (maxX != -1)
                        exTilesIndex.Add(new Vector2(i, maxX));
                }

                //From right to left, duplicate each tile and then clear, move the block one tile to the right side
                for (int i = 0; i < exTilesIndex.Count; i++)
                {
                    int exTileX = (int)exTilesIndex[i].x;
                    int exTileY = (int)exTilesIndex[i].y;

                    for (int j = exTileY; j >= 0; j--)
                    {
                        gridTiles[y + exTileX, j + x + 1].Duplicate(blockTiles[exTileX, j]);
                        gridTiles[y + exTileX, j + x].Clear();
                    }
                }

                x += 1;

                break;
            case Enums.Direction.Down:
                //Run through the tiles from top to bottom, left to right, and add bottom-most extremetiles of each column to the list
                for (int i = 0; i < block.GetWidth(); i++)
                {
                    int maxY = -1; //Record the y index for bottom-most extremtile
                    for (int j = 0; j < block.GetHeight(); j++)
                    {
                        if (blockTiles[j, i].GetIsOccupied())
                        {
                            maxY = j;

                            //Check conditions which stop the block from moving. Return false if there is one condition fulfilled
                            //Three conditions: block on the edge, obstruction inside the block, obstruction outside the block
                            if (y + maxY + 1 > grid.GetHeight() || (maxY + 1 < block.GetHeight() &&
                            !blockTiles[maxY + 1, i].GetIsOccupied() && gridTiles[y + maxY + 1, i].GetIsOccupied())
                            || (maxY + 1 == block.GetHeight() && gridTiles[y + maxY + 1, i].GetIsOccupied()))
                                return false;
                        }
                    }

                    //If max equals to -1, then the column is empty
                    if (maxY != -1)
                        exTilesIndex.Add(new Vector2(maxY, i));
                }

                //From bottom to top, duplicate each tile and then clear, move the block one tile to the down side
                for (int i = 0; i < exTilesIndex.Count; i++)
                {
                    int exTileX = (int)exTilesIndex[i].x;
                    int exTileY = (int)exTilesIndex[i].y;

                    for (int j = exTileX; j >= 0; j--)
                    {
                        gridTiles[y + j + 1, x + exTileY].Duplicate(blockTiles[j, exTileY]);
                        gridTiles[y + j, x + exTileY].Clear();
                    }
                }

                y += 1;

                break;
            case Enums.Direction.Left:
                //Run through the tiles from right to left, top to bottom, and add left-most extremetiles of each row to the list
                for (int i = 0; i < block.GetHeight(); i++)
                {
                    int minX = -1; //Record the x index for left-most extremtile
                    for (int j = block.GetWidth() - 1; j >= 0; j--)
                    {
                        if (blockTiles[i, j].GetIsOccupied())
                        {
                            minX = j;

                            //Check conditions which stop the block from moving. Return false if there is one condition fulfilled
                            //Three conditions: block on the edge, obstruction inside the block, obstruction outside the block
                            if (x + minX - 1 < 0 || (minX - 1 >= 0 &&
                            !blockTiles[i, minX - 1].GetIsOccupied() && gridTiles[i, x + minX - 1].GetIsOccupied())
                            || (minX - 1 == -1 && gridTiles[i, x + minX - 1].GetIsOccupied()))
                                return false;
                        }
                    }

                    //If max equals to -1, then the row is empty
                    if (minX != -1)
                        exTilesIndex.Add(new Vector2(i, minX));
                }

                //From left to right, duplicate each tile and then clear, move the block one tile to the left side
                for (int i = 0; i < exTilesIndex.Count; i++)
                {
                    int exTileX = (int)exTilesIndex[i].x;
                    int exTileY = (int)exTilesIndex[i].y;

                    for (int j = 0; j < exTileY; j++)
                    {
                        gridTiles[y + exTileX, j + x - 1].Duplicate(blockTiles[exTileX, j]);
                        gridTiles[y + exTileX, j + x].Clear();
                    }
                }

                x -= 1;

                break;

            case Enums.Direction.Up:
                //Run through the tiles from bottom to top, left to right, and add top-most extremetiles of each column to the list
                for (int i = 0; i < block.GetWidth(); i++)
                {
                    int minY = -1; //Record the x index for top-most extremtile
                    for (int j = block.GetHeight() - 1; j >= 0; j--)
                    {
                        if (blockTiles[j, i].GetIsOccupied())
                        {
                            minY = j;

                            //Check conditions which stop the block from moving. Return false if there is one condition fulfilled
                            //Three conditions: block on the edge, obstruction inside the block, obstruction outside the block
                            if (y + minY - 1 < 0 || (minY - 1 >= 0 &&
                            !blockTiles[minY - 1, i].GetIsOccupied() && gridTiles[y + minY - 1, i].GetIsOccupied())
                            || (minY - 1 == -1 && gridTiles[y + minY - 1, i].GetIsOccupied()))
                                return false;
                        }
                    }

                    //If max equals to -1, then the column is empty
                    if (minY != -1)
                        exTilesIndex.Add(new Vector2(minY, i));
                }

                //From top to bottom, duplicate each tile and then clear, move the block one tile to the up side
                for (int i = 0; i < exTilesIndex.Count; i++)
                {
                    int exTileX = (int)exTilesIndex[i].x;
                    int exTileY = (int)exTilesIndex[i].y;

                    for (int j = 0; j < exTileX; j++)
                    {
                        gridTiles[y + j - 1, x + exTileY].Duplicate(blockTiles[j, exTileY]);
                        gridTiles[y + j, x + exTileY].Clear();
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
        while (Move(direction))
        {

        }
    }
}
