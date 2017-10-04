//Author(s): Wm. Josiah Erikson
//This class' only purpose is to have the Fade method, which takes an image, 
//fades it over a specified period of time, and then destroys the GameObject
//that it is attached to
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class TileFade : MonoBehaviour
{
    [SerializeField]
    [Tooltip("How long it takes for the tiles to fade when cleared, in seconds")]
    private float fadeTime = 3.0f;
    //UPDATE: Look in Grid.cs for how to use the tuning variable JSON file
	
    public void Fade(Image imageToFade)
    {
        //Set alpha to full to start, so that fade will work
        Color temp = imageToFade.color;
        temp.a = 1.0f;
        imageToFade.color = temp;
        Debug.Log("Fading image over " + fadeTime + "seconds.");
        imageToFade.CrossFadeAlpha(0.0f, fadeTime, true);
        StartCoroutine(WaitToDestroy(imageToFade.gameObject));
    }
    IEnumerator WaitToDestroy (GameObject objectToDestroy) {
        yield return new WaitForSeconds(fadeTime);
        Destroy(objectToDestroy);
    }
}
