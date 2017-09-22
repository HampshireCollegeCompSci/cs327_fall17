// Author(s): Paul Calande, Yixiang Xu

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBlock
{
    //row and col here are the position of the top-left
    //tile in this gridblock, using them as startRow and startCol 
    //to indicate the position of the entire gridblock.
    int row;
    int col;

    Block block;

    Grid grid;

    Tile[,] tiles;

    public GridBlock(int rStart, int cStart, Block myBlock, Grid myGrid)
    {
        row = rStart;
        col = cStart;
        block = myBlock;
        grid = myGrid;
        int width = block.GetWidth();
        int height = block.GetHeight();
        tiles = new Tile[height, width];
        for (int i = 0; i < height; ++i)
        {
            for (int j = 0; j < width; ++j)
            {
                SetTile(i, j, grid.GetTileAt(row + i, col + j));
            }
        }
    }

    //Return the start row index of the gridblock
    public int GetRow()
    {
        return row;
    }

    //Return the start col index of the gridblock
    public int GetCol()
    {
        return col;
    }

    //Set the Tile of given indexes row and col to new Tile
    public void SetTile(int row, int col, Tile newTile)
    {
        // Unsubscribe from the old tile.
        tiles[row, col].Changed -= block.GetCallbackTileDataSetTileType(row, col);
        // Assign new tile.
        tiles[row, col] = newTile;
        // Subscribe to the new tile.
        tiles[row, col].Changed += block.GetCallbackTileDataSetTileType(row, col);
    }

    public void Fill(int row, int col, TileData.TileType newType)
    {
        tiles[row, col].Fill(newType);
    }

    public void Clear(int row, int col)
    {
        tiles[row, col].Clear();
    }

    public bool GetIsOccupied(int row, int col)
    {
        return block.GetIsOccupied(row, col);
    }

    public TileData.TileType GetTileType(int row, int col)
    {
        return block.GetTileType(row, col);
    }

    public bool BelongsToBlock(int row, int col)
    {
        return block.GetIsOccupied(row, col) == grid.GetIsOccupied(row, col);
    }

    /*Repeatedly moves the GridBlock along the Grid in
     *the given direction, one Tile at a time, only stopping
     *at occupied Tiles (preventing Tile overlap) or the
     *border.*/
    public bool Move(Enums.Direction direction)
    {
        //Tile[,] gridTiles = grid.GetTiles();
        //Tile[,] blockTiles = block.GetTiles();
        List<Vector2> exTilesIndex = new List<Vector2>();

        //The order to run through each tile of the block will be different according to direction
        switch (direction)
        {
            case Enums.Direction.Right:
                //Run through the tiles from left to right, top to bottom, and add right-most extremetiles of each row to the list
                for (int i = 0; i < block.GetHeight(); i++)
                {
                    int maxCol = -1; //Record the col index for right-most extremtile
                    for (int j = 0; j < block.GetWidth(); j++)
                    {
                        if (grid.GetIsOccupied(i, j))
                        {
                            maxCol = j;

                            //Check conditions which stop the block from moving. Return false if there is one condition fulfilled
                            //Three conditions: block on the edge, obstruction inside the block, obstruction outside the block
                            if (col + maxCol + 1 > grid.GetWidth() || (maxCol + 1 < block.GetWidth() &&
                            !block.GetIsOccupied(i, maxCol + 1) && grid.GetIsOccupied(i, col + maxCol + 1))
                            || (maxCol + 1 == block.GetWidth() && grid.GetIsOccupied(i, col + maxCol + 1)))
                                return false;
                        }
                    }

                    //If max equals to -1, then the row is empty
                    if (maxCol != -1)
                        exTilesIndex.Add(new Vector2(i, maxCol));
                }

                //From right to left, duplicate each tile and then clear, move the block one tile to the right side
                for (int i = 0; i < exTilesIndex.Count; i++)
                {
                    int exTileX = (int)exTilesIndex[i].x;
                    int exTileY = (int)exTilesIndex[i].y;

                    for (int j = exTileY; j >= 0; j--)
                    {
                        //gridTiles[y + exTileX, j + x + 1].Duplicate(blockTiles[exTileX, j]);
                        grid.Fill(row + exTileX, j + col + 1, block.GetTileType(exTileX, j));
                        SetTile(row + exTileX, j + col + 1, grid.GetTileAt(row + exTileX, j + col + 1));
                        grid.Clear(row + exTileX, j + col);
                    }
                }

                col += 1;

                break;
            case Enums.Direction.Down:
                //Run through the tiles from top to bottom, left to right, and add bottom-most extremetiles of each column to the list
                for (int i = 0; i < block.GetWidth(); i++)
                {
                    int maxY = -1; //Record the y index for bottom-most extremtile
                    for (int j = 0; j < block.GetHeight(); j++)
                    {
                        if (block.GetIsOccupied(j, i))
                        {
                            maxY = j;

                            //Check conditions which stop the block from moving. Return false if there is one condition fulfilled
                            //Three conditions: block on the edge, obstruction inside the block, obstruction outside the block
                            if (row + maxY + 1 > grid.GetHeight() || (maxY + 1 < block.GetHeight() &&
                            !block.GetIsOccupied(maxY + 1, i) && grid.GetIsOccupied(row + maxY + 1, i)
                            || (maxY + 1 == block.GetHeight() && grid.GetIsOccupied(row + maxY + 1, i))))
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
                        //gridTiles[y + j + 1, x + exTileY].Duplicate(blockTiles[j, exTileY]);
                        grid.Fill(row + j + 1, col + exTileY, block.GetTileType(j, exTileY));
                        SetTile(row + j + 1, col + exTileY, grid.GetTileAt(row + j + 1, col + exTileY));
                        grid.Clear(row + j, col + exTileY);
                    }
                }

                row += 1;

                break;
            case Enums.Direction.Left:
                //Run through the tiles from right to left, top to bottom, and add left-most extremetiles of each row to the list
                for (int i = 0; i < block.GetHeight(); i++)
                {
                    int minX = -1; //Record the x index for left-most extremtile
                    for (int j = block.GetWidth() - 1; j >= 0; j--)
                    {
                        if (block.GetIsOccupied(i, j))
                        {
                            minX = j;

                            //Check conditions which stop the block from moving. Return false if there is one condition fulfilled
                            //Three conditions: block on the edge, obstruction inside the block, obstruction outside the block
                            if (col + minX - 1 < 0 || (minX - 1 >= 0 &&
                            !block.GetIsOccupied(i, minX - 1) && grid.GetIsOccupied(i, col + minX - 1)
                            || (minX - 1 == -1 && grid.GetIsOccupied(i, col + minX - 1))))
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
                        //gridTiles[y + exTileX, j + x - 1].Duplicate(blockTiles[exTileX, j]);
                        grid.Fill(row + exTileX, j + col - 1, block.GetTileType(exTileX, j));
                        SetTile(row + exTileX, j + col - 1, grid.GetTileAt(row + exTileX, j + col - 1));
                        grid.Clear(row + exTileX, j + col);
                    }
                }

                col -= 1;

                break;

            case Enums.Direction.Up:
                //Run through the tiles from bottom to top, left to right, and add top-most extremetiles of each column to the list
                for (int i = 0; i < block.GetWidth(); i++)
                {
                    int minY = -1; //Record the x index for top-most extremtile
                    for (int j = block.GetHeight() - 1; j >= 0; j--)
                    {
                        if (block.GetIsOccupied(j, i))
                        {
                            minY = j;

                            //Check conditions which stop the block from moving. Return false if there is one condition fulfilled
                            //Three conditions: block on the edge, obstruction inside the block, obstruction outside the block
                            if (row + minY - 1 < 0 || (minY - 1 >= 0 &&
                            !block.GetIsOccupied(minY - 1, i) && grid.GetIsOccupied(row + minY - 1, i)
                            || (minY - 1 == -1 && grid.GetIsOccupied(row + minY - 1, i))))
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
                        //gridTiles[y + j - 1, x + exTileY].Duplicate(blockTiles[j, exTileY]);
                        grid.Fill(row + j - 1, col + exTileY, block.GetTileType(j, exTileY));
                        SetTile(row + j - 1, col + exTileY, grid.GetTileAt(row + j - 1, col + exTileY));
                        grid.Clear(row + j, col + exTileY);
                    }
                }

                row -= 1;

                break;
        }

        return true;
    }

    //Runs the Move method with the given direction over and over until Move returns false
    public void MoveRepeatedly(Enums.Direction direction)
    {
        while (Move(direction))
        {
            // No need to put anything in here.
        }
    }

    //Check all the tiles of the gridblock
    //If a tile is the type of Vacant, then it will be cleared
    //If a tile is the type of Vestige, then a 1x1 gridblock will be created
    //and the new gridblock will fall to the bottom of the grid or be obstructed
    //by other tiles and stop right there
    public void ActivateVestiges()
    {
        for (int r = 0; r <block.GetHeight(); r++)
        {
            for (int c = 0; c <block.GetWidth(); c++)
            {
                if (GetTileType(r, c) == TileData.TileType.Vacant)
                {
                    Clear(r, c);
                }
                else if (GetTileType(r, c) == TileData.TileType.Regular)
                {
                    Block b = new Block(1, 1);
                    b.Fill(0, 0, TileData.TileType.Vestige);
                    GridBlock gb = grid.WriteBlock(row + r, col + c, b);
                    gb.MoveRepeatedly(Enums.Direction.Down);
                }
            }
        }
    }
}