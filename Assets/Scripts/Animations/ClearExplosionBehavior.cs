using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearExplosionBehavior : MonoBehaviour {

	public void DestroyMe()
    {
        Destroy(gameObject);
    }
}
