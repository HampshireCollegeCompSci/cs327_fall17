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

	// Use this for initialization
	void Start () {
        var json = JSON.Parse(tipJSON.ToString());
        tips = json["tips"].AsArray;
        GetComponent<Text>().text = "23333333333";
        Debug.Log(tips);
        //tips[Random.Range(0, tips.Count)]
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
