using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIButton : MonoBehaviour {

    public BlockSpawner Spawner;
	
    public void RotateClockwise()
    {
        Spawner.RotateCurrentBlock(false);  
    }

    public void RotateCounter()
    {
        Spawner.RotateCurrentBlock(true);
    }

    public void Reset()
    {
        SceneManager.LoadScene(1);
    }

}
