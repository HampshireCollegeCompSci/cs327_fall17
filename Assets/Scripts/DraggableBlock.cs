// Author(s): Paul Calande

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableBlock : MonoBehaviour
{
    Block block;
    bool isDraggable = false;

    Tile[,] tiles;
    GameObject prefabTile;
    Grid grid;
    DraggableObject draggableObject;

    //Copies the data of a Block into this DraggableBlock’s contained Block
    public void SetBlock(Block copiedBlock)
    {
        tiles = TileUtil.CreateTileArray(prefabTile, transform.position, copiedBlock.GetWidth(), copiedBlock.GetHeight());
        block = new Block(copiedBlock.GetWidth(), copiedBlock.GetHeight());
        for (int r = 0; r < block.GetHeight(); r++)
        {
            for (int c = 0; c < block.GetWidth(); c++)
            {
                block.Fill(r, c, copiedBlock.GetTileType(r, c));
            }
        }
        
        UpdateTiles();
        UpdateAvailableSpaces();
    }

    public Block GetBlock()
    {
        return block;
    }

    public void AllowDragging()
    {
        isDraggable = true;
    }

    //Retrieve a List of Spaces that the DraggableBlock can fit into and store that List as draggableObject’s 
    //List of SnapLocations. Use DraggableObject.SetSnapToAreas to pass in the List of SnapLocations.
    void UpdateAvailableSpaces()
    {
        List<Space> spaces = grid.GetSpacesFree(block.GetWidth(),block.GetHeight(), block);
        List<SnapLocation> sl = new List<SnapLocation>();
        for (int i = 0; i < spaces.Count; i++)
        {
            sl.Add(spaces[i].GetComponent<SnapLocation>());
        }

        draggableObject.SetSnapToAreas(sl);
    }

    //Forwards a Rotate call to the underlying Block, then updates the tiles 
    //array to match the Block’s TileData. Lastly, call UpdateAvailableSpaces.
    public void Rotate(bool clockwise)
    {
        block.Rotate(clockwise);
        UpdateTiles();
        UpdateAvailableSpaces();
    }

    public void UpdateTiles()
    {
        for (int r = 0; r < block.GetHeight(); r++)
        {
            for (int c = 0; c < block.GetWidth(); c++)
            {
                tiles[r, c].Fill(block.GetTileType(r, c));
            }
        }
    }
}