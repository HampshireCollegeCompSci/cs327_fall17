// Author(s): Paul Calande
// Script that disables the gameObject based on the status of a hardcoded static variable.
// Used for disabling cheat-related objects (like cheat buttons) when the game is ready to
// be shipped.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatElement : MonoBehaviour
{
    static bool cheatsExist = false;

    private void Start()
    {
        if (!cheatsExist)
        {
            gameObject.SetActive(false);
        }
    }
}