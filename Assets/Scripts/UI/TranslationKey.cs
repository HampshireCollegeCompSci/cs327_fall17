// Author(s): Paul Calande, Maia Doerner

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TranslationKey : MonoBehaviour
{
    [SerializeField]
    string translationKey;

    UILanguages translator = null;

    Text text = null;

    void Start()
    {
        Translate();
    }

    public void Translate()
    {
        if (text == null)
        {
            text = GetComponent<Text>();
        }
        if (translator == null)
        {
            translator = FindObjectOfType<UILanguages>();
        }

        text.text = translator.Translate(translationKey);
    }
}