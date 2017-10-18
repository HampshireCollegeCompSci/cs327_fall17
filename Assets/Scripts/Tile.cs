// Author(s): Paul Calande, Maia Doerner, Wm. Josiah Erikson

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    public delegate void ChangedHandler(TileData.TileType type);
    public event ChangedHandler Changed;

    [SerializeField]
    [Tooltip("Reference to the Image component to use for rendering the Tile.")]
    Image spriteRenderer;
    [SerializeField]
    [Tooltip("The sprite to use for Unoccupied state.")]
    Sprite spriteUnoccupied;
    [SerializeField]
    [Tooltip("The sprites to use for Vestige state.")]
    Sprite[] spriteVestigeList;
    [SerializeField]
    [Tooltip("The fading tile prefab to instantiate.")]
    Transform fadingTilePrefab;
    [SerializeField]
    [Tooltip("The sprite of regular Tile.")]
    Sprite[] tiles;
    [SerializeField]
    [Tooltip("The current Sprite of the Tile chosen in Fill. Does not necessarily match the current SpriteRenderer sprite.")]
    Sprite trueSprite;

    // The underlying TileData.
    TileData data = new TileData();

    private void Awake()
    {
        SetSprite(data.GetTileType());
        trueSprite = spriteRenderer.sprite;
    }

    public bool GetIsOccupied()
    {
        return data.GetIsOccupied();
    }

    public void Fill(TileData.TileType newType)
    {
        TileData.TileType previousType = GetTileType();
        if (previousType != newType)
        {
            // Before we actually change the tile type, check and handle special cases.
            if (newType == TileData.TileType.Unoccupied)
            {
                // Create the fading visual effects if the Tile hasn't already been cleared.
                if (previousType != TileData.TileType.Unoccupied)
                {
                    CreateVanishVisualEffects();
                }
            }

            data.Fill(newType);

            SetSprite(newType);
            trueSprite = spriteRenderer.sprite;

            OnChanged(newType);
        }
    }

    public TileData.TileType GetTileType()
    {
        return data.GetTileType();
    }

    public void Clear()
    {
        // Change this tile to unoccupied
        Fill(TileData.TileType.Unoccupied);
        
    }

    // Called when the Tile is occupied and gets cleared.
    private void CreateVanishVisualEffects()
    {
        GameObject gridObject = transform.parent.gameObject; //Get a handle on the grid
        Transform grid = gridObject.transform; //For readability, get its Transform
                                               //Make a new prefab instance to fade out
        Transform thisFadingTilePrefab = Instantiate(fadingTilePrefab, grid.transform.position, grid.transform.rotation);
        thisFadingTilePrefab.SetParent(grid, false);
        thisFadingTilePrefab.transform.localPosition = gameObject.transform.localPosition;
        //Copy the sprite to the instantiated prefab so that it fades out the same sprite that was cleared
        thisFadingTilePrefab.GetComponent<Image>().sprite = trueSprite;
        //Get the fade component
        TileFade tileToFade = thisFadingTilePrefab.GetComponent<TileFade>();
        tileToFade.Fade(); //And fade the image out, which will destroy it as well
        gridObject.GetComponent<ClearedSquaresCounter>().ClearedSquare(); //increment the total number of cleared squares, for analytics
    }

    // Helper function.
    public void Duplicate(Tile other)
    {
        Fill(other.GetTileType());
    }

    public void SetSprite(TileData.TileType newType)
    {
        // Set the sprite based on the given tile type.
        Sprite newSprite = null;
        switch (newType)
        {
            case TileData.TileType.Unoccupied:
                newSprite = spriteUnoccupied;
                break;
            case TileData.TileType.Regular:
                int randomInt = Random.Range(0, tiles.Length);
                newSprite = tiles[randomInt];
                break;
            /*
            case TileData.TileType.Vacant:
                newSprite = spriteVacant;
                break;
            */
            case TileData.TileType.Vestige:
                newSprite = spriteVestigeList[0];
                break;
        }
        spriteRenderer.sprite = newSprite;
    }

    public void SetSprite(Sprite newSprite)
    {
        spriteRenderer.sprite = newSprite;
    }

    public void SetSpriteToTrueSprite()
    {
        spriteRenderer.sprite = trueSprite;
    }

    public void SetSpriteAbsolute(Sprite newSprite)
    {
        spriteRenderer.sprite = newSprite;
        trueSprite = newSprite;
    }

    public void EnableSpriteRenderer(bool enable)
    {
        spriteRenderer.enabled = enable;
    }

    public void SetHighlight()
    {
        spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
    }

    public void SetAnticipatedHighlight(TileData.TileType type)
    {
        if (type == TileData.TileType.Regular)
            spriteRenderer.color = new Color(0.0f, 0.0f, 1.0f, 0.5f);
        else if (type == TileData.TileType.Vestige)
            spriteRenderer.color = new Color(1.0f, 0.0f, 0.0f, 0.5f);
    }

    public void SetNormal()
    {
        spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }

    public Sprite GetSprite()
    {
        return spriteRenderer.sprite;
    }

    void OnChanged(TileData.TileType newType)
    {
        if (Changed != null)
        {
            Changed(newType);
        }
    }

    public void SetVestigeLevel(int level)
    {
        Sprite newSprite = spriteVestigeList[level - 1];
        spriteRenderer.sprite = newSprite;
        trueSprite = newSprite;

        data.SetVestigeLevel(level);
    }

    public int GetVestigeLevel()
    {
        return data.GetVestigeLevel();
    }
}