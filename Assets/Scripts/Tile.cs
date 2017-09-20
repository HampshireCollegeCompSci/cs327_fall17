// Author(s): Paul Calande, Maia Doerner


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
	Sprite spriteFilled;
	Sprite empty;
	SpriteRenderer spriteRenderer;


    public bool GetIsOccupied()
    {
        return isOccupied;
    }
		
    public void Fill()
    {
		//this.type = TileType.Regular;
		this.spriteRenderer.sprite = spriteFilled;
		this.isOccupied = true;
    }

    public void Clear()
    {
		//Tile.Destroy;
		this.spriteRenderer.sprite = empty;
		this.isOccupied = false;
    }

    public void Duplicate(Tile other)
    {
		this.isOccupied = other.isOccupied;
		this.spriteRenderer.sprite = other.spriteRenderer.sprite;
		this.type = other.type;
    }

    public void SetType(TileType newType)
    {
		this.type = newType;

    }

    public TileType GetTileType()
    {
        return type;
    }
}