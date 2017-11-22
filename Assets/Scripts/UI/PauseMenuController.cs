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
    [SerializeField]
    [Tooltip("Reference to the lose progress warning.")]
    GameObject LoseProgressWarning;

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

    public void Warning()
    {
        LoseProgressWarning.SetActive(true);
        AudioController.Instance.MenuClick();
    }

    public void CancelExit()
    {
        LoseProgressWarning.SetActive(false);
        AudioController.Instance.MenuClick();
    }
}
