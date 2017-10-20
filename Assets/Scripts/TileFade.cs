//Author: Wm. Josiah Erikson
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
    [Tooltip("Reference to the Image component to fade.")]
    Image imageToFade;
    [SerializeField]
    [Tooltip("How many seconds the tile takes to fade. Populated by Remote Variables.")]
    public float fadeTime;
    [SerializeField]
    [Tooltip("Reference to the tile clearing prefab.")]
    GameObject prefabTileClearing;

	public void Fade()
    {
        //Set alpha to full to start, so that fade will work
        Color temp = imageToFade.color;
        temp.a = 1.0f;
        imageToFade.color = temp;
        imageToFade.CrossFadeAlpha(0.0f, fadeTime, true);
        StartCoroutine(WaitToDestroy(imageToFade.gameObject));

        //Clearing animation
        Instantiate(prefabTileClearing, transform, false);
        //tileClear.GetComponent<Animator>().enabled = true;
    }
    IEnumerator WaitToDestroy (GameObject objectToDestroy) {
        yield return new WaitForSeconds(fadeTime);
        Destroy(objectToDestroy);
    }
}
