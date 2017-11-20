using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeFont : MonoBehaviour {

	[SerializeField]
	[Tooltip ("the fallbackk font")]
	Font FallbackFont;

	UILanguages translator;

	void Awake() {
		translator = FindObjectOfType<UILanguages>();
	}
	// Use this for initialization
	void Start () {
		if(translator.GetLanguage() == "Romanian"||translator.GetLanguage() == "Slovenian"||translator.GetLanguage() == "Polish") {
			GetComponent<Text>().font = FallbackFont;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
