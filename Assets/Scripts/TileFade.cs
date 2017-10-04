//Author: Wm. Josiah Erikson
//This class' only purpose is to have the Fade method, which takes an image, 
//fades it over a specified period of time, and then destroys the GameObject
//that it is attached to
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using SimpleJSON;


public class TileFade : MonoBehaviour
{
	[SerializeField]
	TextAsset tuningJSON;
    private float fadeTime;

	private void Tune()
	{
        //Read the tuning variables in from the JSON file
		var json = JSON.Parse(tuningJSON.ToString());
        fadeTime = json["cleared cell fade time"].AsFloat;
	}

		public void Fade(Image imageToFade)
    {
        Tune(); //Start doesn't get called right away for some reason, so we tune here.
        //Set alpha to full to start, so that fade will work
        Color temp = imageToFade.color;
        temp.a = 1.0f;
        imageToFade.color = temp;
        imageToFade.CrossFadeAlpha(0.0f, fadeTime, true);
        StartCoroutine(WaitToDestroy(imageToFade.gameObject));
    }
    IEnumerator WaitToDestroy (GameObject objectToDestroy) {
        yield return new WaitForSeconds(fadeTime);
        Destroy(objectToDestroy);
    }
}
