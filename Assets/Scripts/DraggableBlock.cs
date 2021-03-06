﻿// Author(s): Paul Calande, Yixiang Xu

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableBlock : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The prefab to use to instantiate each Tile.")]
    GameObject prefabTile;
    [SerializeField]
    [Tooltip("Reference to the Grid instance.")]
    Grid grid;
    [SerializeField]
    [Tooltip("Reference to the Console Grid instance.")]
    ConsoleGrid consoleGrid;
    [SerializeField]
    [Tooltip("Reference to the DraggableObject component to interface with.")]
    DraggableObject draggableObject;
    [SerializeField]
    [Tooltip("The size of the draggable block while being dragged.")]
    Vector3 draggingScale;
    [SerializeField]
    [Tooltip("The size of the draggable block while not being dragged.")]
    Vector3 nonDraggingScale;

    // The TileData forming the DraggableBlock.
    Block block;

    // The underlying array of Tiles forming the DraggableBlock.
    Tile[,] tiles;

    //Copies the data of a Block into this DraggableBlock’s contained Block
    public void Init(Block copiedBlock, Grid newGrid, RectTransform canvas, ConsoleGrid newConsoleGrid)
    {
        grid = newGrid;
        consoleGrid = newConsoleGrid;
        //consoleGrid.Init();
        draggableObject.SetCanvasTransform(canvas);

        //nonDraggingScale = new Vector3(0.75f, 0.75f, 0.75f);
        nonDraggingScale = new Vector3(1f, 1f, 1f);
        //draggingScale = new Vector3(1.0f, 1.0f, 1.0f);

        float scaleVlaueY = grid.GetTileHeight() / consoleGrid.GetTileHeight();
        float scaleVlaueX = grid.GetTileWidth() / consoleGrid.GetTileWidth();
        draggingScale = new Vector3(scaleVlaueX, scaleVlaueY, 1.0f);

        draggableObject.SetDraggingScale(draggingScale);
        draggableObject.SetNonDraggingScale(nonDraggingScale);
        draggableObject.BeginDragEvent += DraggableObject_BeginDragEvent;

        //transform.localScale = nonDraggingScale;

        transform.SetParent(consoleGrid.transform);

        // Copy the copiedBlock data into this DraggableBlock's block.
        block = new Block(copiedBlock);
        /*
        block = new Block(copiedBlock.GetHeight(), copiedBlock.GetWidth());
        for (int r = 0; r < block.GetHeight(); r++)
        {
            for (int c = 0; c < block.GetWidth(); c++)
            {
                block.Fill(r, c, copiedBlock.GetTileType(r, c));

                //tiles[r, c].GetComponent<RectTransform>().SetParent(transform);
            }
        }
        */
        
        UpdateTiles();
    }

    private void OnDestroy()
    {
        draggableObject.BeginDragEvent -= DraggableObject_BeginDragEvent;
    }

    public Block GetBlock()
    {
        return block;
    }

    /*
    public void SetScreenTapping(ScreenTapping tapping)
    {
        draggableObject.SetScreenTapping(tapping);
    }
    */

    public void AllowDragging(bool draggable)
    {
        draggableObject.SetIsDraggable(draggable);
    }

    //Retrieve a List of Spaces that the DraggableBlock can fit into and store that List as draggableObject’s 
    //List of SnapLocations. Use DraggableObject.SetSnapToAreas to pass in the List of SnapLocations.
    public void UpdateAvailableSpaces()
    {
        //List<Space> spaces = grid.GetSpacesFree(block.GetWidth(),block.GetHeight(), block);
        List<Space> spaces = grid.GetSpacesFree(1, 1, block);
        List<SnapLocation> sl = new List<SnapLocation>();
        for (int i = 0; i < spaces.Count; i++)
        {
            sl.Add(spaces[i].GetComponent<SnapLocation>());
        }

        draggableObject.SetSnapToAreas(sl);
    }

    // Forwards a Rotate call to the underlying Block, then updates the tiles 
    // array to match the Block’s TileData.
    public void Rotate(bool clockwise)
    {
        block.Rotate(clockwise);
        UpdateTiles();
    }

    // Forwards a Flip call to the underlying Block, then updates the tiles 
    // array to match the Block’s TileData.
    public void Flip()
    {
        block.Flip();
        UpdateTiles();
    }

    public void UpdateTiles()
    {
        // Destroy the Tiles so that they can be recreated.
        if (tiles != null)
        {
            foreach (Tile t in tiles)
            {
                Destroy(t.gameObject);
            }
        }

        int height = block.GetHeight();
        int width = block.GetWidth();

        // Re-initialize the tiles array.
        tiles = CreateTileArrayForConsole(Vector3.zero, height, width);

        // Fill in all of the Tiles according to the Block.
        for (int r = 0; r < height; r++)
        {
            for (int c = 0; c < width; c++)
            {
                tiles[r, c].Duplicate(block.GetTileData(r, c));
                tiles[r, c].SetSpriteToTrueSprite();
                //tiles[r, c].Fill(block.GetTileType(r, c));
                //tiles[r, c].SetVestigeLevel(block.GetVestigeLevel(r, c));
                /*
                // Unoccupied Tiles should not be visible.
                if (tiles[r, c].GetTileType() == TileData.TileType.Unoccupied)
                {
                    //tiles[r, c].EnableSpriteRenderer(false);
                }
                */
            }
        }
        UpdateAvailableSpaces();

        // Calculate the snap detection offset based on the width and height of the block.
        float tileWidth = grid.GetTileWidth();
        float tileHeight = grid.GetTileHeight();
        float xoff = (width - 1) * 0.5f * tileWidth;
        float yoff = (height - 1) * 0.5f * tileHeight;
        Vector2 offset = new Vector2(-xoff, yoff);
        draggableObject.SetSnapDetectionOffset(offset);

        //Debug.Log("DraggableBlock width / height: " + width + " / " + height);
        //Debug.Log("tileWidth / tileHeight: " + tileWidth + " / " + tileHeight);
        //Debug.Log("DraggableBlock snapDetectionOffset: " + offset);
    }

    public Tile[,] CreateTileArrayForConsole(Vector3 center, int blockHeight, int blockWidth)
    {
        Tile[,] result = new Tile[blockHeight, blockWidth];

        int gridWidth = consoleGrid.GetWidth();
        int gridHeight = consoleGrid.GetHeight();

        float tileWidth = consoleGrid.GetTileWidth();
        float tileHeight = consoleGrid.GetTileHeight();

        // Calculate the position of the top-left corner of the array.
        float cornerX = center.x - ((gridWidth - 1) * tileWidth * 0.5f);
        float cornerY = center.y + ((gridHeight - 1) * tileHeight * 0.5f);

        //Debug.Log(tileWidth);

        int rowStart = (gridHeight - blockHeight) / 2;
        int colStart = (gridWidth - blockWidth) / 2;

        // Iterate through all the Tiles of the array.
        for (int c = colStart; c < colStart + blockWidth; c++)
        {
            for (int r = rowStart; r < rowStart + blockHeight; r++)
            {
                // Calculate the position of the Tile.
                float tileX = cornerX + c * tileWidth;
                float tileY = cornerY - r * tileHeight;
                Vector3 pos = new Vector3(tileX, tileY, 0.0f);

                GameObject currentPrefabTile = GameObject.Instantiate(prefabTile, transform, false);
                currentPrefabTile.transform.localPosition = pos;

                result[r - rowStart, c - colStart] = currentPrefabTile.GetComponent<Tile>();

                currentPrefabTile.GetComponent<RectTransform>().sizeDelta = new Vector2(tileWidth, tileHeight);
            }
        }

        return result;
    }

    public void SetDefaultPosition(Vector2 pos)
    {
        draggableObject.SetDefaultPosition(pos);
    }

    public int GetWidth()
    {
        return block.GetWidth();
    }

    public int GetHeight()
    {
        return block.GetHeight();
    }

    public bool GetIsOccupied(int row, int col)
    {
        return block.GetIsOccupied(row, col);
    }

    public Sprite GetSprite(int row, int col)
    {
        return tiles[row, col].GetSprite();
    }

    public TileData.TileType GetTileType(int row, int col)
    {
        return tiles[row, col].GetTileType();
    }

    public TileData GetTileData(int row, int col)
    {
        return block.GetTileData(row, col);
    }

	public void TurnBlockImageOff() 
	{
		int height = block.GetHeight();
		int width = block.GetWidth();
		for (int r = 0; r < height; r++)
		{
			for (int c = 0; c < width; c++)
			{
				tiles [r, c].EnableSpriteRenderer(false);
			}
		}
	}

	public void TurnBlockImageOn()
	{
		int height = block.GetHeight();
		int width = block.GetWidth();
		for (int r = 0; r < height; r++)
		{
			for (int c = 0; c < width; c++)
			{
				tiles [r, c].EnableSpriteRenderer(true);
			}
		}
	}

    /*
    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(transform.position, new Vector3(10.0f, 10.0f, 10.0f) * 10.0f);
    }
    */

    private void DraggableObject_BeginDragEvent(DraggableObject drag)
    {
        AudioController.Instance.PickupTile();
    }
}