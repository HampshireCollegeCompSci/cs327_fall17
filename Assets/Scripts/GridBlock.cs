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

    Tile[,] tiles;

    public GridBlock(int xStart, int yStart, Block myBlock, Grid myGrid)
    {
        x = xStart;
        y = xStart;
        block = myBlock;
        grid = myGrid;
        int width = block.GetWidth();
        int height = block.GetHeight();
        tiles = new Tile[width, height];
        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                SetTile(i, j, grid.GetTileAt(x + i, y + j));
            }
        }
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

    //Set the Tile of given indexes x and y to new Tile
    public void SetTile(int x, int y, Tile newTile)
    {
        // Unsubscribe from the old tile.
        tiles[x, y].Changed -= block.GetCallbackTileDataSetTileType(x, y);
        // Assign new tile.
        tiles[x, y] = newTile;
        // Subscribe to the new tile.
        tiles[x, y].Changed += block.GetCallbackTileDataSetTileType(x, y);
    }

    public void Fill(int x, int y)
    {
        tiles[x, y].Fill();
    }

    public void Clear(int x, int y)
    {
        tiles[x, y].Clear();
    }

    public bool GetIsOccupied(int x, int y)
    {
        return block.GetIsOccupied(x, y);
    }

    public TileData.TileType GetTileType(int x, int y)
    {
        return block.GetTileType(x, y);
    }

    public void SetTileType(int x, int y, TileData.TileType type)
    {
        tiles[x, y].SetTileType(type);
    }

    public bool BelongsToBlock(int x, int y)
    {
        return block.GetIsOccupied(x, y) == grid.GetIsOccupied(x, y);
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
                    int maxX = -1; //Record the x index for right-most extremtile
                    for (int j = 0; j < block.GetWidth(); j++)
                    {
                        if (grid.GetIsOccupied(i, j))
                        {
                            maxX = j;

                            //Check conditions which stop the block from moving. Return false if there is one condition fulfilled
                            //Three conditions: block on the edge, obstruction inside the block, obstruction outside the block
                            if (x + maxX + 1 > grid.GetWidth() || (maxX + 1 < block.GetWidth() &&
                            !block.GetIsOccupied(i, maxX + 1) && grid.GetIsOccupied(i, x + maxX + 1))
                            || (maxX + 1 == block.GetWidth() && grid.GetIsOccupied(i, x + maxX + 1)))
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
                        //gridTiles[y + exTileX, j + x + 1].Duplicate(blockTiles[exTileX, j]);
                        grid.SetTileType(y + exTileX, j + x + 1, block.GetTileType(exTileX, j));
                        SetTile(y + exTileX, j + x + 1, grid.GetTileAt(y + exTileX, j + x + 1));
                        grid.Clear(y + exTileX, j + x);
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
                        if (block.GetIsOccupied(j, i))
                        {
                            maxY = j;

                            //Check conditions which stop the block from moving. Return false if there is one condition fulfilled
                            //Three conditions: block on the edge, obstruction inside the block, obstruction outside the block
                            if (y + maxY + 1 > grid.GetHeight() || (maxY + 1 < block.GetHeight() &&
                            !block.GetIsOccupied(maxY + 1, i) && grid.GetIsOccupied(y + maxY + 1, i)
                            || (maxY + 1 == block.GetHeight() && grid.GetIsOccupied(y + maxY + 1, i))))
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
                        grid.SetTileType(y + j + 1, x + exTileY, block.GetTileType(j, exTileY));
                        SetTile(y + j + 1, x + exTileY, grid.GetTileAt(y + j + 1, x + exTileY));
                        grid.Clear(y + j, x + exTileY);
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
                        if (block.GetIsOccupied(i, j))
                        {
                            minX = j;

                            //Check conditions which stop the block from moving. Return false if there is one condition fulfilled
                            //Three conditions: block on the edge, obstruction inside the block, obstruction outside the block
                            if (x + minX - 1 < 0 || (minX - 1 >= 0 &&
                            !block.GetIsOccupied(i, minX - 1) && grid.GetIsOccupied(i, x + minX - 1)
                            || (minX - 1 == -1 && grid.GetIsOccupied(i, x + minX - 1))))
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
                        grid.SetTileType(y + exTileX, j + x - 1, block.GetTileType(exTileX, j));
                        SetTile(y + exTileX, j + x - 1, grid.GetTileAt(y + exTileX, j + x - 1));
                        grid.Clear(y + exTileX, j + x);
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
                        if (block.GetIsOccupied(j, i))
                        {
                            minY = j;

                            //Check conditions which stop the block from moving. Return false if there is one condition fulfilled
                            //Three conditions: block on the edge, obstruction inside the block, obstruction outside the block
                            if (y + minY - 1 < 0 || (minY - 1 >= 0 &&
                            !block.GetIsOccupied(minY - 1, i) && grid.GetIsOccupied(y + minY - 1, i)
                            || (minY - 1 == -1 && grid.GetIsOccupied(y + minY - 1, i))))
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
                        grid.SetTileType(y + j - 1, x + exTileY, block.GetTileType(j, exTileY));
                        SetTile(y + j - 1, x + exTileY, grid.GetTileAt(y + j - 1, x + exTileY));
                        grid.Clear(y + j, x + exTileY);
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
                    b.SetTileType(0, 0, TileData.TileType.Vestige);
                    GridBlock gb = grid.WriteBlock(y + r, x + c, b);
                    gb.MoveRepeatedly(Enums.Direction.Down);
                }
            }
        }
    }
}