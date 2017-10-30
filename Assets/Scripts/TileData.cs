// Author(s): Paul Calande, Maia Doerner

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileData
{
	public enum TileType
	{
        Uninitialized,
        Unoccupied,
		Regular,
		Vestige,
        Asteroid
	}

    [SerializeField]
    [Tooltip("The TileType of this TileData.")]
    TileType type;
    [SerializeField]
    [Tooltip("The index of the sprite in the relevant sprites array, if applicable.")]
    int spriteIndex = 0;
    [SerializeField]
    [Tooltip("The level of the vestige, if applicable. 0 for all non-vestige tiles.")]
    int vestigeLevel = 0;

    // Default constructor.
    public TileData()
    {
        type = TileType.Uninitialized;
    }

    public TileData(TileType newType)
    {
        type = newType;
    }

    // Copy constructor.
    public TileData(TileData other)
    {
        type = other.type;
        vestigeLevel = other.vestigeLevel;
        spriteIndex = other.spriteIndex;
    }

    public void Clear()
    {
        //type = TileType.Unoccupied;
        Fill(TileType.Unoccupied);
    }

    public void Fill(TileType newType)
    {
        //vestigeLevel = 0;
        type = newType;
    }

    public bool GetIsOccupied()
    {
        return type != TileType.Unoccupied;
    }

    public TileType GetTileType()
    {
        return type;
    }

    public void SetVestigeLevel(int level)
    {
        vestigeLevel = level;
    }

    public int GetVestigeLevel()
    {
        return vestigeLevel;
    }

    public bool GetIsClearableInSquare()
    {
        return GetIsClearableInSquare(type);
    }

    public void SetSpriteIndex(int index)
    {
        spriteIndex = index;
    }

    public int GetSpriteIndex()
    {
        return spriteIndex;
    }

    // Returns true if the given TileType is clearable.
    // This method essentially lists all of the clearable TileTypes.
    public static bool GetIsClearableInSquare(TileData.TileType type)
    {
        return type == TileType.Regular ||
            type == TileType.Vestige;
    }
}