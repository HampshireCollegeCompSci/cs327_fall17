// Author(s): Paul Calande, Maia Doerner

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileData
{
	public enum TileType
	{
        Unoccupied,
		Regular,
		Vestige
	}

    TileType type;
    int vestigeLevel;

    // Default constructor.
    public TileData()
    {
        type = TileType.Unoccupied;
        vestigeLevel = 0; //0 for all non-vestige tiles
    }

    public TileData(TileType newType)
    {
        type = newType;
    }

    // Copy constructor.
    public TileData(TileData other)
    {
        type = other.type;
    }

    public void Clear()
    {
        type = TileType.Unoccupied;
    }

    public void Fill(TileType newType)
    {
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
}