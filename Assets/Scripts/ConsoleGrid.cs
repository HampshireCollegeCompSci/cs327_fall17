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
    [Tooltip("The underlying array of Tiles.")]
    Tile[,] tiles;
    [SerializeField]
    [Tooltip("Reference to the main grid.")]
    Grid grid;
    [SerializeField]
    [Tooltip("Reference to the current draggableBlock in the grid.")]
    DraggableBlock draggableBlock;
    [SerializeField]
    [Tooltip("The prefab to use to instantiate each Tile.")]
    GameObject prefabTile;

    // The width of one Tile, calculated compared to the Grid's dimensions.
    float tileWidth;
    // The height of one Tile, calculated compared to the Grid's dimensions.
    float tileHeight;

    void Tune()
    {
        var json = JSON.Parse(tuningJSON.ToString());
        width = json["console grid width"].AsInt;
        height = json["console grid height"].AsInt;
        //Debug.Log(width);
    }

    // Use this for initialization
    void Start () {
        
        //Debug.Log(GetComponent<RectTransform>().rect.width);

        //Instantiate tiles array
        /*tiles = CreateTileArray(prefabTile, Vector3.zero, height, width);

        foreach (Tile t in tiles)
        {
            // Make all of the Tiles clear.
            t.Clear();
            t.SetSprite(TileData.TileType.Unoccupied);
        }*/
    }

    public void Init()
    {
        Tune();
        tileWidth = GetComponent<RectTransform>().rect.width / (float)width;
        tileHeight = GetComponent<RectTransform>().rect.height / (float)height;
        //Debug.Log(tileWidth);
    }

    public void SetDraggableBlock(DraggableBlock block)
    {
        draggableBlock = block;
    }
	
	// Update is called once per frame
	private void Update () {
		
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

    public Tile[,] GetTiles()
    {
        return tiles;
    }

    public void OnDrag(PointerEventData eventData)
    {
        draggableBlock.GetComponent<DraggableObject>().OnDrag(eventData);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        draggableBlock.GetComponent<DraggableObject>().OnBeginDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        draggableBlock.GetComponent<DraggableObject>().OnEndDrag(eventData);
    }
}
