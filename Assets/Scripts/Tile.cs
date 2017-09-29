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
        Fill(TileData.TileType.Unoccupied);
        //Make a new tile a bit in front of this one, fade it out

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

    void OnChanged(TileData.TileType newType)
    {
        if (Changed != null)
        {
            Changed(newType);
        }
    }
}