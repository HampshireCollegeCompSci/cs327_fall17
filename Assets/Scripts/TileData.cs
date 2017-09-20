using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileData : MonoBehaviour {

	public bool isOccupied;
	public enum TileType
	{
		Regular,
		Vacant,
		Vestige
	}
	public TileType type;
}
