// Author(s): Joel Esquilin, Paul Calande

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is to be filled with device-specific instructions and methods. 
public class DeviceUtils : MonoBehaviour
{
    private void Awake()
    {
        // Have the GameObject persist between scenes, but prevent duplication.
        // Make sure that no other GameObject with this name exists.
        if (GameObject.Find(this.name) == this.gameObject)
        {
            //Debug.Log("No other " + name + " GameObject found!");
            DontDestroyOnLoad(transform.gameObject);

            Screen.orientation = ScreenOrientation.Portrait;
            Screen.autorotateToLandscapeLeft = false;
            Screen.autorotateToLandscapeRight = false;
            Screen.autorotateToPortrait = true;
            Screen.autorotateToPortraitUpsideDown = true;
        }
        else
        {
            Destroy(gameObject);
        }
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