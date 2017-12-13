using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenshotScript: MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //if (Input.GetMouseButton(0)) {
        //    ScreenCapture.CaptureScreenshot("Screenshot-" + Time.time + ".png");
        //    Debug.Log("Captured Screenshot");
        //}
		
	}
    public void TakeScreenshot(){
        ScreenCapture.CaptureScreenshot("Screenshot-" + Time.time + ".png");
        Debug.Log("Captured Screenshot");
    }
    //void OnMouseDown()
    //{
    //    ScreenCapture.CaptureScreenshot("Screenshot.png");
    //    Debug.Log("Captured Screenshot");
    //}
}
