using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyMeter : MonoBehaviour {
    private RectTransform rT; //For storing the rectTransform component

	// Use this for initialization
	void Start () {
        rT = GetComponent<RectTransform>(); //This only happens once and prevents things from breaking
        rT.sizeDelta = new Vector2(rT.sizeDelta.x, rT.sizeDelta.y + 0.5f);		
	}
	
	// Update is called once per frame
	void Update () {
        //Vector3 ourLocalPosition = transform.localPosition;
        //rT.sizeDelta = new Vector2(rT.sizeDelta.x, rT.sizeDelta.y + 0.5f);
        float yPosition = transform.localPosition.y;
        yPosition += 2.0f;
        float xPosition = transform.localPosition.x;
        transform.localPosition = new Vector2(xPosition, yPosition);
        //transform.localPosition.Set(yPosition,xPosition,zPosition); //This doesn't work - don't know why. Have to use world space. Seems bad.
        //        transform.Translate(Vector3.up * 20);
        //Quaternion currentRotation = transform.localRotation;
        //Vector3 newPosition = new Vector3(xPosition, yPosition, zPosition);
        //transform.SetPositionAndRotation(newPosition, currentRotation);
	}
}
