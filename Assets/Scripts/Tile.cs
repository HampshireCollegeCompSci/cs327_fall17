// Author(s): Paul Calande, Maia Doerner


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public delegate void ChangedHandler(TileData.TileType type);
    public event ChangedHandler Changed;

    //static Dictionary<TileData.TileType, Sprite> sprites = new Dictionary<TileData.TileType, Sprite>();

	TileData data;

    [SerializeField]
	SpriteRenderer spriteRenderer;
    [SerializeField]
    Sprite spriteUnoccupied;
    [SerializeField]
    Sprite spriteRegular;
    [SerializeField]
    Sprite spriteVacant;
    [SerializeField]
    Sprite spriteVestige;

    public bool GetIsOccupied()
    {
        return data.GetIsOccupied();
    }

    public void SetTileType(TileData.TileType newType)
    {
        data.SetTileType(newType);
        // Set the sprite based on the new tile type.
        switch (newType)
        {
            case TileData.TileType.Unoccupied:
                spriteRenderer.sprite = spriteUnoccupied;
                break;
            case TileData.TileType.Regular:
                spriteRenderer.sprite = spriteRegular;
                break;
            case TileData.TileType.Vacant:
                spriteRenderer.sprite = spriteVacant;
                break;
            case TileData.TileType.Vestige:
                spriteRenderer.sprite = spriteVestige;
                break;
        }
        OnChanged(newType);
    }

    public TileData.TileType GetTileType()
    {
        return data.GetTileType();
    }

    public void Clear()
    {
        SetTileType(TileData.TileType.Unoccupied);
    }

    public void Fill()
    {
        SetTileType(TileData.TileType.Regular);
    }

    // Helper function.
    public void Duplicate(Tile other)
    {
        SetTileType(other.GetTileType());
    }

    void OnChanged(TileData.TileType newType)
    {
        if (Changed != null)
        {
            Changed(newType);
        }
    }
}