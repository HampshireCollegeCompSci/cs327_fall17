// Author(s): Paul Calande

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneReset : MonoBehaviour
{

    public GameObject confirmPanel;

    public void ResetScene()
    {

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void OpenMenu()
    {
        AudioController.Instance.MenuClick();

        confirmPanel.SetActive(true);
    }

    public void Cancel()
    {
        AudioController.Instance.MenuClick();

        confirmPanel.SetActive(false);
    }
}
