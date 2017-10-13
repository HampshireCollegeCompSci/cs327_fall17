// Author(s): Paul Calande, Maia Doerner, Wm. Josiah Erikson

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    public delegate void ChangedHandler(TileData.TileType type);
    public event ChangedHandler Changed;

    //static Dictionary<TileData.TileType, Sprite> sprites = new Dictionary<TileData.TileType, Sprite>();

	TileData data = new TileData();

    [SerializeField]
    [Tooltip("Reference to the Image component to use for rendering the Tile.")]
    Image spriteRenderer;
    [SerializeField]
    [Tooltip("The sprite to use for Unoccupied state.")]
    Sprite spriteUnoccupied;
    [SerializeField]
    [Tooltip("The sprite to use for Regular state.")]
   	Sprite spriteRegular;
    [SerializeField]
    [Tooltip("The sprite to use for Vestige state.")]
    Sprite spriteVestige;
    [SerializeField]
    [Tooltip("The fading tile prefab to instantiate.")]
    Transform fadingTilePrefab;
	[SerializeField]
	[Tooltip("The sprite of regular Tile.")]
	Sprite Tile_Regular;
	[SerializeField]
	[Tooltip("The sprite of Normal 2 Tile.")]
	Sprite Tile_Normal2;
	[SerializeField]
	[Tooltip("The sprite of Normal 3 Tile.")]
	Sprite Tile_Normal3;

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
        //Get the fade component
        TileFade tileToFade = thisFadingTilePrefab.GetComponent<TileFade>();
        //Image imageToFade = thisFadingTilePrefab.GetComponent<Image>();
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
        switch (newType)
        {
            case TileData.TileType.Unoccupied:
                spriteRenderer.sprite = spriteUnoccupied;
                break;
			case TileData.TileType.Regular:
				int randomInt = Random.Range (0, 2);				
				if(randomInt == 0) 
				{
				spriteRenderer.sprite = Tile_Regular;
				}
				if(randomInt == 1)
				{
				spriteRenderer.sprite = Tile_Normal2;
				}
				if(randomInt == 2)
				{
				spriteRenderer.sprite = Tile_Normal3;
				}
				//spriteRenderer.sprite = spriteRegular;
                break;
                /*
            case TileData.TileType.Vacant:
                spriteRenderer.sprite = spriteVacant;
                break;
                */
            case TileData.TileType.Vestige:
                spriteRenderer.sprite = spriteVestige;
                break;
        }
    }

    public void EnableSpriteRenderer(bool enable)
    {
        spriteRenderer.enabled = enable;
    }

    public void SetHighlight()
    {
        spriteRenderer.color = new Color(255f, 255f, 255f, 0.5f);
    }

    public void SetAnticipatedHighlight(TileData.TileType type)
    {
        if (type == TileData.TileType.Regular)
            spriteRenderer.color = new Color(0f, 0f, 255f, 0.5f);
        else if (type == TileData.TileType.Vestige)
            spriteRenderer.color = new Color(255f, 0f, 0f, 0.5f);
    }

    public void SetNormal()
    {
        spriteRenderer.color = new Color(255f, 255f, 255f, 1f);
    }

    public void SetIncomingHighlight(TileData.TileType type)
    {
        if (type == TileData.TileType.Regular)
            spriteRenderer.color = new Color(0f, 0f, 255f, 1f);
        else if (type == TileData.TileType.Vestige)
            spriteRenderer.color = new Color(255f, 0f, 0f, 1f);
    }


    void OnChanged(TileData.TileType newType)
    {
        if (Changed != null)
        {
            Changed(newType);
        }
    }
}