using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;

public class Tip : MonoBehaviour
{

    [SerializeField]
    [Tooltip("Reference to the Tuning JSON.")]
    TextAsset tipJSON;
    [SerializeField]
    [Tooltip("The list of tips could be shown in the game over screen. Populated by JSON.")]
    JSONArray tips;

    [SerializeField]
    [Tooltip("Reference to the Tuning JSON.")]
    TextAsset tuningJSON;

    UILanguages translator;

    string displayTip;

    // Use this for initialization
    void Start()
    {
        translator = FindObjectOfType<UILanguages>();
        var json = JSON.Parse(tipJSON.ToString());
        tips = json["tips"].AsArray;

        var tjson = JSON.Parse(tuningJSON.ToString());
        bool randomized = tjson["randomize tips"].AsBool;

        int tipsCount = 0;

        if (randomized)
            tipsCount = Random.Range(0, tips.Count);
        else
            tipsCount = PlayerPrefs.GetInt("Current Tip", 0);

        displayTip = tips[tipsCount];
        displayTip = translator.Translate(displayTip);
        GetComponent<Text>().text = displayTip;
    }

    private void OnDestroy()
    {
        PlayerPrefs.SetInt("Current Tip", (PlayerPrefs.GetInt("Current Tip", 0) + 1) % tips.Count);
    }
}
