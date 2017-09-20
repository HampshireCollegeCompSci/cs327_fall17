// Author(s): Paul Calande, Maia Doerner


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    bool isOccupied = false;

	Sprite spriteFilled;
	Sprite empty;
	SpriteRenderer spriteRenderer;
	TileData data;

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
		data.type = data.type;
    }

	public void SetType(TileData.TileType newType)
    {
		data.type = newType;

    }

	public TileData.TileType GetTileType()
    {
        return data.type;
    }
}