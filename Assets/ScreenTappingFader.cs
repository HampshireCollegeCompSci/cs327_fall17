using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenTappingFader : MonoBehaviour {

	float fadeTime = .8f;
	Image image;
	Color imageColor;
	float randomRotate;
	float randomPosition;
	float speed;
	Vector3 direction;

	void Awake () 
	{
		//fadetTime = 1f;
		image = GetComponent<Image>();
		imageColor = image.color;
		StartCoroutine (FadeOut (image));
		randomRotate = Random.Range(0, 5);
		randomPosition = Random.Range(0, 50);
		float x = Random.Range(-1f, 1f);
		float y = Random.Range(-1f, 1f);
		direction = new Vector3(x, y, 0f);
		speed = 5f;
	}

	void Update () 
	{
		
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
			Debug.Log (imageColor);
			if (imageColor.a == 0) {
				Destroy (gameObject);
			}
			transform.Rotate(Time.deltaTime * 5, Time.deltaTime * 5, 0);
			transform.position += direction * speed;

		}
	} 

}
