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
    Dictionary<int, string> languageDictionary = new Dictionary<int, string>();

    private void Awake()
    {
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
    }

    void Start()
    {
        translator = FindObjectOfType<UILanguages>();

        if (languages != null)
        {
            languages.onValueChanged.AddListener(ChangeLanguage);
        }
    }

    public void ChangeLanguage(int value)
    {
        translator.SetLanguage(languageDictionary[value]);
    }
}