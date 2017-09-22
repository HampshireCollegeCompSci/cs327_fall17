using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIButton : MonoBehaviour {
    public BlockSpawner spawner;
    public GameFlow gameFlow;

    public bool resetButton;
    public Image image;
    public Text text;

    private void Start()
    {
        if (gameFlow != null && resetButton)
        {
            gameFlow.GameLost += Appear;
        }
        
    }

    public void RotateClockwise()
    {
        spawner.RotateCurrentBlock(true);  
    }

    public void RotateCounter()
    {
        spawner.RotateCurrentBlock(false);
    }

    public void Reset()
    {
        SceneManager.LoadScene(1);
    }

    void Appear()
    {
        if (image != null)
        {
            image.enabled = true;
        }

        if (text != null)
        {
            text.enabled = true;
        }

    }
}
