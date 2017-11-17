// Author(s): Yifeng Shi, Paul Calande

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactorBreachBehavior : MonoBehaviour {

    Tile tile;
    Sprite sprite;

    public void ReferenceTile(Tile thisTile, Sprite spriteAsteroid)
    {
        tile = thisTile;
        sprite = spriteAsteroid;
    }

    public void DestroyMe()
    {
        Destroy(gameObject);
        tile.NullifyAsteroid();
        tile.SetSpriteAbsolute(sprite);
    }

    public void DestroyMePrematurely()
    {
        //Debug.Log("RBB.DestroyMePrematurely");
        Destroy(gameObject);
        tile.NullifyAsteroid();
        tile.Clear();
    }
}
