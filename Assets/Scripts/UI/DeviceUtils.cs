// Author(s): Joel Esquilin

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is to be filled with device-specific instructions and methods. 
public class DeviceUtils : MonoBehaviour
{
    void Update()
    {
        // The Android back button is typically tied to the escape key.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}