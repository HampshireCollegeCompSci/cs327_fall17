// Author(s): Paul Calande
// Helper class for destroying an animation GameObject when it finishes animating.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyMeUtil : MonoBehaviour
{
    public void DestroyMe()
    {
        Destroy(gameObject);
    }
}