//Author(s) : Maia Doerner
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class UILanguages : MonoBehaviour {

	[SerializeField]
	[Tooltip("Reference to the Translation JSON.")]
	TextAsset translationsJSON;
	[SerializeField]
	[Tooltip("Dropdown language menu")]
	Dropdown languages;

    // English by default.
	string gameLanguage = "English";

	UnityAction<int> onValue;

	public static UILanguages instance = null;

	public void Start() 
	{
		onValue += ChangeLanguage;
        if (languages != null)
        {
            languages.onValueChanged.AddListener(onValue);
        }
	}

	void Awake () {
		if (instance) {
			DestroyImmediate(gameObject);
			return;
		}
		instance = this;
		DontDestroyOnLoad(gameObject);
	}

	public string Translate(string descriptor)
	{
		var json = JSON.Parse(translationsJSON.ToString());
		JSONNode textToTranslate = json [descriptor];
		string translation = textToTranslate [gameLanguage];
        Debug.Log("descriptor / gameLanguage: " + descriptor + " / " + gameLanguage);
		return translation;
	}

	public void SetLanguage(string language) 
	{
		gameLanguage = language;
	}

	public void ChangeLanguage(int value)
	{
		if (value == 0) 
		{
			SetLanguage ("English");
		}
		if (value == 1) 
		{
			SetLanguage ("Spanish");
		}
		if (value == 2) 
		{
			SetLanguage ("French");
		}
		if (value == 3)
		{
			SetLanguage ("Slovenian");
		}
		if (value == 4) 
		{
			SetLanguage ("Romanian");
		}
		if (value == 5) 
		{
			SetLanguage ("German");
		}
		if (value == 6) 
		{
			SetLanguage ("Luxembourgish");
		}
		if (value == 7) 
		{
			SetLanguage ("Dutch");
		}
		if (value == 8) 
		{
			SetLanguage ("Danish");
		}
		if (value == 9) 
		{
			SetLanguage ("Polish");
		}
		if (value == 10) 
		{
			SetLanguage ("Korean");
		}
	}



}
