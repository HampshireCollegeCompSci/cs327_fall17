using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatMenuController : MonoBehaviour {

    [SerializeField]
    [Tooltip("Reference of cheat menu")]
    GameObject cheatMenu;

    public void OpenCheetMenu()
    {
        cheatMenu.SetActive(true);
    }

    public void GoBackToPause()
    {
        cheatMenu.SetActive(false);
    }
}
