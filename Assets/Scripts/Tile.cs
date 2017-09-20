// Author(s): Paul Calande, [your name here]

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    bool isOccupied = false;

    public enum TileType
    {
        Regular,
        Vacant,
        Vestige
    }

    TileType type;

    public bool GetIsOccupied()
    {
        return isOccupied;
    }

    public void Fill()
    {

    }

    public void Clear()
    {

    }

    public void Duplicate(Tile other)
    {

    }

    public void SetType(TileType newType)
    {

    }

    public TileType GetTileType()
    {
        return type;
    }
}