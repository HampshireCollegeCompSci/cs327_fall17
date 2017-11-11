using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenTappingFader : MonoBehaviour {
	/*
	[SerializeField]
	[Tooltip("The amount of time the fade animation takes")]
	float fadeTime;
	[SerializeField]
	[Tooltip("Image ")]
	Image image;
	[SerializeField]
	[Tooltip("")]
	Color imageColor;
	*/
	float fadeTime = .8f;
	Image image;
	Color imageColor;
	// Use this for initialization
	void Start () 
	{
		//fadetTime = 1f;
		image = GetComponent<Image>();
		imageColor = image.color;
		StartCoroutine (FadeOut (image));

	}

	private YieldInstruction fadeInstruction = new YieldInstruction();
	IEnumerator FadeOut(Image image)
	{
		float elapsedTime = 0.0f;
		while (elapsedTime < fadeTime)
		{
			yield return fadeInstruction;
			elapsedTime += Time.deltaTime ;
			imageColor.a = 1.0f - Mathf.Clamp01(elapsedTime / fadeTime);
			image.color = imageColor;
			//Debug.Log (imageColor);
			if (imageColor.a == 0) {
				Destroy (gameObject);
			}
		}
	} 

}
