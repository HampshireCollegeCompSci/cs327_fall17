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
    private float fadeTime = 0.4f;
    //UPDATE: Look in Grid.cs for how to use the tuning variable JSON file
	
    public void Fade(Image imageToFade)
    {
        imageToFade.CrossFadeAlpha(0, fadeTime, false);
        Destroy(imageToFade.gameObject);
    }
}
