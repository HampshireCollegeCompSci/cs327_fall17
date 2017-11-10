// Author(s): Paul Calande, Maia Doerner

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LanguagesDropdown : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Dropdown language menu.")]
    Dropdown languages;

    UILanguages translator;

    // Integers mapped to languages.
    //Dictionary<int, string> languageDictionary = new Dictionary<int, string>();
    List<string> languageList = new List<string>();

    private void Awake()
    {
        /*
        languageDictionary.Add(0, "English");
        languageDictionary.Add(1, "Spanish");
        languageDictionary.Add(2, "French");
        languageDictionary.Add(3, "Slovenian");
        languageDictionary.Add(4, "Romanian");
        languageDictionary.Add(5, "German");
        languageDictionary.Add(6, "Luxembourgish");
        languageDictionary.Add(7, "Dutch");
        languageDictionary.Add(8, "Danish");
        languageDictionary.Add(9, "Polish");
        languageDictionary.Add(10, "Korean");
        */
        languageList.Add("English");
        languageList.Add("Spanish");
        languageList.Add("French");
        languageList.Add("Slovenian");
        languageList.Add("Romanian");
        languageList.Add("German");
        languageList.Add("Luxembourgish");
        languageList.Add("Dutch");
        languageList.Add("Danish");
        languageList.Add("Polish");
        languageList.Add("Korean");
    }

    void Start()
    {
        translator = FindObjectOfType<UILanguages>();

        if (languages != null)
        {
            languages.onValueChanged.AddListener(ChangeLanguage);
        }

        languages.value = languageList.IndexOf(translator.GetLanguage());
    }

    public void ChangeLanguage(int value)
    {
        translator.SetLanguage(languageList[value]);
    }
}