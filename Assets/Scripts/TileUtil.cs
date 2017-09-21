// Author(s): Paul Calande, Yifeng Shi

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileUtil
{
    public static Tile[,] CreateTileArray(GameObject prefabTile, Vector3 center, int width, int height)
    {
        Tile[,] result = new Tile[width, height];

        const float spaceBetweenTiles = 0.32f;

        // Calculate the position of the top-left corner of the array.
        float topLeftX = center.x - ((width - 1) * spaceBetweenTiles * 0.5f);
        float topLeftY = center.y - ((height - 1) * spaceBetweenTiles * 0.5f);

        // Iterate through all the Tiles of the array.
        for (int c = 0; c < width; c++)
        {
            for (int r = 0; r < height; r++)
            {
                // Calculate the position of the Tile.
                float tileX = topLeftX + c * spaceBetweenTiles;
                float tileY = topLeftY + r * spaceBetweenTiles;
                Vector3 pos = new Vector3(tileX, tileY, 0.0f);

                // Instantiate the Tile!
                GameObject currentPrefabTile = GameObject.Instantiate(prefabTile, pos, Quaternion.identity);
                result[r, c] = currentPrefabTile.GetComponent<Tile>();
            }
        }

        return result;
    }
}