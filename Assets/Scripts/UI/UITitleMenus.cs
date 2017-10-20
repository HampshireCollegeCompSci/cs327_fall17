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
        Time.timeScale = 1f;
    }

    public void GoToHowToPlay()
    {
        SceneManager.LoadScene("HowToPlay");
    }

    public void GoToAbout()
    {
        SceneManager.LoadScene("About");
    }
	public void GoToGamePlay() 
	{
		SceneManager.LoadScene ("MainScene");
	}
    public void GoToCheatPage()
    {
        SceneManager.LoadScene("CheatPage");
    }
}