﻿// Author(s): Paul Calande, Yixiang Xu

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
    }

    public void GoToHowToPlay()
    {
        SceneManager.LoadScene("HowToPlay");
    }

	public void GoToSettings()
	{
		SceneManager.LoadScene("Settings");
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