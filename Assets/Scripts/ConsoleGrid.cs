// Author(s): Paul Calande, Yixiang Xu
// Script for the console grid that new DraggableBlocks spawn in.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using UnityEngine.EventSystems;

public class ConsoleGrid : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField]
    [Tooltip("Reference to the Tuning JSON.")]
    TextAsset tuningJSON;
    [SerializeField]
    [Tooltip("The width of the Grid. Populated by JSON.")]
    int width;
    [SerializeField]
    [Tooltip("The height of the Grid. Populated by JSON.")]
    int height;
    [SerializeField]
    [Tooltip("Reference to the main grid.")]
    Grid grid;
    /*
    [SerializeField]
    [Tooltip("The underlying array of Tiles.")]
    Tile[,] tiles;
    [SerializeField]
    [Tooltip("Reference to the current draggableBlock in the grid.")]
    DraggableBlock draggableBlock;
    */
    [SerializeField]
    [Tooltip("Reference to the DraggableBlock's DraggableObject.")]
    DraggableObject draggableObject;
    [SerializeField]
    [Tooltip("The prefab to use to instantiate each Tile.")]
    GameObject prefabTile;
    [SerializeField]
    [Tooltip("Reference to the RectTransform component.")]
    RectTransform rt;

    // The width of one Tile, calculated compared to the Grid's dimensions.
    float tileWidth;
    // The height of one Tile, calculated compared to the Grid's dimensions.
    float tileHeight;

    // Reference to the underlying block.
    Block block;

    void Tune()
    {
        var json = JSON.Parse(tuningJSON.ToString());
        width = json["console grid width"].AsInt;
        height = json["console grid height"].AsInt;
    }

    public void Init()
    {
        Tune();
        tileWidth = rt.rect.width / (float)width;
        tileHeight = rt.rect.height / (float)height;
    }

    public void SetDraggableBlock(DraggableBlock blockIn)
    {
        //draggableBlock = block;
        if (blockIn != null)
        {
            draggableObject = blockIn.GetComponent<DraggableObject>();
            enabled = true;
            block = blockIn.GetBlock();
        }
        else
        {
            draggableObject = null;
            enabled = false;
        }
    }

    public float GetTileWidth()
    {
        return tileWidth;
    }

    public float GetTileHeight()
    {
        return tileHeight;
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    public List<TileData> GetVestiges()
    {
        if (draggableObject == null)
        {
            return null;
        }
        else
        {
            return block.GetReferencesToType(TileData.TileType.Vestige);
        }
    }

    /*
    public Tile[,] GetTiles()
    {
        return tiles;
    }
    */

    public void OnDrag(PointerEventData eventData)
    {
        draggableObject.OnDrag(eventData);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        draggableObject.OnBeginDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        draggableObject.OnEndDrag(eventData);
    }
}