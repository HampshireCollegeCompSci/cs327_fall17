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

    TileType type = TileType.Unoccupied;

    public void Clear()
    {
        type = TileType.Unoccupied;
    }

    public void Fill()
    {
        type = TileType.Regular;
    }

    public bool GetIsOccupied()
    {
        return type != TileType.Unoccupied;
    }

    public void SetTileType(TileType newType)
    {
        type = newType;
    }

    public TileType GetTileType()
    {
        return type;
    }
}