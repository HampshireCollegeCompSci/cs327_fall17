using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockInBag : MonoBehaviour {

    [SerializeField]
    [Tooltip("The tile prefab to instantiate.")]
    GameObject prefabTile;

    public void Init(Block block, float cellX, float cellY, float tileHeight, float tileWidth)
    {
        Tile[,] tiles = new Tile[block.GetHeight(), block.GetWidth()];

        float height = block.GetHeight();
        float width = block.GetWidth();
        float scaleX = 0.5f * cellX / tileWidth;
        float scaleY = 0.5f * cellY / tileHeight;

        float cornerX = - ((width - 1) * tileWidth * 0.5f);
        float cornerY = ((height - 1) * tileHeight * 0.5f);

        for (int c = 0; c < width; c++)
        {
            for (int r = 0; r < height; r++)
            {
                float tileX = cornerX / 2 + c * tileWidth * scaleX;
                float tileY = cornerY / 2 - r * tileHeight * scaleY;
                Vector3 pos = new Vector3(tileX, tileY, 0.0f);

                GameObject currentPrefabTile = GameObject.Instantiate(prefabTile, transform, false);
                currentPrefabTile.transform.localPosition = pos;
                currentPrefabTile.transform.localScale = new Vector3(scaleX, scaleY, 1.0f);
                tiles[r, c] = currentPrefabTile.GetComponent<Tile>();
            }
        }

        for (int c = 0; c < width; c++)
        {
            for (int r = 0; r < height; r++)
            {
                tiles[r, c].Duplicate(block.GetTileData(r, c));
            }
        }

        
    }
}
