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
		Vacant,
		Vestige
	}

    TileType type;

    // Default constructor.
    public TileData()
    {
        type = TileType.Unoccupied;
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
}