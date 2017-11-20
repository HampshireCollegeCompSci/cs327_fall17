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
    [SerializeField]
    GameObject blocksInBagPanel;

    public void OpenPauseMenu()
    {
        pauseMenu.SetActive(true);
        AudioController.Instance.MenuClick();
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        AudioController.Instance.MenuClick();
    }

    public void ShowBlocksInBag()
    {
        blocksInBagPanel.GetComponent<ShowBlocksInBag>().ShowBlocks();
    }
}
