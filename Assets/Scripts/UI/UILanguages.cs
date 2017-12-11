// Author(s): Paul Calande, Maia Doerner
// Translation and language-tracking script for the Translator singleton object.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class UILanguages : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Reference to the Translation JSON.")]
    TextAsset translationsJSON;

	[SerializeField]
	[Tooltip("Font for languages with advance characters")]
	Font fontBackUp;

    string gameLanguage;

    public static UILanguages instance = null;

    void Awake()
    {
        if (instance)
        {
            DestroyImmediate(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        // Read the language from the player preferences.
        // Make the language be English by default.
        gameLanguage = PlayerPrefs.GetString("Language", "English");
    }

    public string Translate(string descriptor)
    {
        var json = JSON.Parse(translationsJSON.ToString());
        JSONNode textToTranslate = json[descriptor];
        string translation = textToTranslate[gameLanguage];
        //Debug.Log("descriptor / gameLanguage: " + descriptor + " / " + gameLanguage);
        /*
		if (gameLanguage == "Polish" || gameLanguage == "Romanian" || gameLanguage == "Slovenian") 
		{
			textMesh.font = fontBackUp;
			textMesh.renderer.sharedMaterial = ArialFont.material;
		}
        */
		return translation;
    }

    public void SetLanguage(string language)
    {
        gameLanguage = language;
        PlayerPrefs.SetString("Language", gameLanguage);

        // Update the text meshes in the scene.
        TranslationKey[] keys = FindObjectsOfType<TranslationKey>();
        foreach (TranslationKey key in keys)
        {
            key.Translate();
        }
    }
	/*
	public bool IsLanguageThatNeedsNewFont () {
		if (gameLanguage == "Polish" || gameLanguage == "Romanian" || gameLanguage == "Slovenian") 
		{
			return true;
		} 
		else 
		{
			return false;
		}
	}

	public Font Font() {
		return fontBackUp;
	}
	*/
    public string GetLanguage()
    {
        return gameLanguage;
    }
}