using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TranslationKey : MonoBehaviour {
	[SerializeField]
	string translationKey;

	Button button;

	UILanguages translator;

	void Start () {
		translator = FindObjectOfType<UILanguages>();
		Button ();
	}

	public void Button() {
		GetComponent<Text> ().text = translator.Translate (translationKey);
	}

}
