// Author(s): Yixiang Xu

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuController : MonoBehaviour {
    [SerializeField]
    [Tooltip("Reference to the pause menu.")]
    GameObject pauseMenu;
    [SerializeField]
    [Tooltip("Reference to the screen tapping controller.")]
    ScreenTapping screenTapping;

    public void OpenPauseMenu()
    {
        pauseMenu.SetActive(true);
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
    }
}
