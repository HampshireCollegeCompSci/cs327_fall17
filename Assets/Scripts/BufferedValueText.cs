// Author(s): Paul Calande
// Updates text based on a buffered value.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BufferedValueText : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Reference to the BufferedValue component.")]
    BufferedValue bufferedValue;
    [SerializeField]
    [Tooltip("Reference to the Text to change.")]
    Text text;

    private void Awake()
    {
        bufferedValue.ValueUpdated += BufferedValue_ValueUpdated;
    }

    private void BufferedValue_ValueUpdated(int newValue, int difference)
    {
        text.text = newValue.ToString();
    }
}