using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;

public class Tip : MonoBehaviour {

    [SerializeField]
    [Tooltip("Reference to the Tuning JSON.")]
    TextAsset tipJSON;
    [SerializeField]
    [Tooltip("The list of tips could be shown in the game over screen. Populated by JSON.")]
    JSONArray tips;

	UILanguages translator;

	string displayTip;

	// Use this for initialization
	void Start () {
		translator = FindObjectOfType<UILanguages>();
        var json = JSON.Parse(tipJSON.ToString());
        tips = json["tips"].AsArray;
		displayTip = tips [Random.Range (0, tips.Count)];
		//displayTip = translator.Translate (displayTip);
		GetComponent<Text>().text = displayTip;
	}
}
