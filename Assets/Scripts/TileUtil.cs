// Author(s): Paul Calande, Yifeng Shi

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileUtil
{
    // The amount of space between each Tile in a grid of Tiles.
    public const float spaceBetweenTiles = 64.0f;

    public static Tile[,] CreateTileArray(GameObject prefabTile, Transform parent, Vector3 center, int height, int width)
    {
        Tile[,] result = new Tile[height, width];

        // Calculate the position of the top-left corner of the array.
        float cornerX = center.x - ((width - 1) * spaceBetweenTiles * 0.5f);
        float cornerY = center.y + ((height - 1) * spaceBetweenTiles * 0.5f);

        // Iterate through all the Tiles of the array.
        for (int c = 0; c < width; c++)
        {
            for (int r = 0; r < height; r++)
            {
                // Calculate the position of the Tile.
                float tileX = cornerX + c * spaceBetweenTiles;
                float tileY = cornerY - r * spaceBetweenTiles;
                Vector3 pos = new Vector3(tileX, tileY, 0.0f);

                GameObject currentPrefabTile = Object.Instantiate(prefabTile, parent, false);
                currentPrefabTile.transform.localPosition = pos;

                /*
                if (c == 0 && r == 0)
                {
                    // Let's figure out which Tile ends up in the (0, 0) corner...
                    currentPrefabTile.transform.localScale = new Vector3(2.0f, 2.0f, 2.0f);
                }
                */

                result[r, c] = currentPrefabTile.GetComponent<Tile>();
            }
        }

        return result;
    }
}