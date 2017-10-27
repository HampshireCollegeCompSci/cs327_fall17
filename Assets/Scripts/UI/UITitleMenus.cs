// Author(s): Paul Calande, Yixiang Xu

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UITitleMenus : MonoBehaviour
{
	public void GoToTitle()
    {
        SceneManager.LoadScene("Title");
        AudioController.Instance.MenuClick();
    }

    public void GoToHowToPlay()
    {
        SceneManager.LoadScene("HowToPlay");
        AudioController.Instance.MenuClick();
    }

    public void GoToAbout()
    {
        SceneManager.LoadScene("About");
        AudioController.Instance.MenuClick();
    }
	public void GoToGamePlay() 
	{
		SceneManager.LoadScene ("MainScene");
        AudioController.Instance.MenuClick();
    }
    public void GoToCheatPage()
    {
        SceneManager.LoadScene("CheatPage");
        AudioController.Instance.MenuClick();
    }
}