using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeviceUtils : MonoBehaviour {

    //This class is to be filled with device specific instructions and methods. 
    //The android back button is typically tied to the escape key.

    void Update () {
		if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
	}
}
