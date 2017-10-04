// Author(s): Paul Calande, Maia Doerner

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
    Image spriteRenderer;
    [SerializeField]
    Sprite spriteUnoccupied;
    [SerializeField]
    Sprite spriteRegular;
    //[SerializeField]
    //Sprite spriteVacant;
    [SerializeField]
    Sprite spriteVestige;
    [SerializeField]
    [Tooltip("The fading tile prefab")]
    Transform fadingTilePrefab;
    TileFade tileToFade;

    public bool GetIsOccupied()
    {
        return data.GetIsOccupied();
    }

    public void Fill(TileData.TileType newType)
    {
        TileData.TileType previousType = data.GetTileType();
        if (previousType != newType)
        {
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
        Fill(TileData.TileType.Unoccupied); //Change this tile to unoccupied
        GameObject gridObject = transform.parent.gameObject;
        Transform grid = gridObject.transform;
        Transform thisFadingTilePrefab = Instantiate(fadingTilePrefab, grid.transform.position, grid.transform.rotation);
        thisFadingTilePrefab.SetParent(grid, false);
        thisFadingTilePrefab.transform.localPosition = gameObject.transform.localPosition;
        //Get the fade component
        tileToFade = thisFadingTilePrefab.GetComponent<TileFade>();
        Image imageToFade = thisFadingTilePrefab.GetComponent<Image>();
        tileToFade.Fade(imageToFade); //And fade the image out, which should destroy it as well
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
                spriteRenderer.sprite = spriteRegular;
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

    public void SetNormal()
    {
        spriteRenderer.color = new Color(255f, 255f, 255f, 1f);
    }

    void OnChanged(TileData.TileType newType)
    {
        if (Changed != null)
        {
            Changed(newType);
        }
    }
}