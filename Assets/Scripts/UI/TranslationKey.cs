using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TranslationKey : MonoBehaviour
{
    [SerializeField]
    string translationKey;

    UILanguages translator;

    void Start()
    {
        translator = FindObjectOfType<UILanguages>();
        GetComponent<Text>().text = translator.Translate(translationKey);
    }
}