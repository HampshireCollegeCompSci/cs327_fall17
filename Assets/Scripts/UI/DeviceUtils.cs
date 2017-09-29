// Author(s): Joel Esquilin

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is to be filled with device-specific instructions and methods. 
public class DeviceUtils : MonoBehaviour
{
    private void Awake()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        Screen.autorotateToLandscapeLeft = false;
        Screen.autorotateToLandscapeRight = false;
        Screen.autorotateToPortrait = true;
        Screen.autorotateToPortraitUpsideDown = true;
    }
    void Update()
    {
        // The Android back button is typically tied to the escape key.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}