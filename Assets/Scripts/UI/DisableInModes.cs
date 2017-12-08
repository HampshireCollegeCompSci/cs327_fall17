// Author(s): Paul Calande
// Disables gameObject in certain game modes.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableInModes : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Whether this object is enabled in the tutorial.")]
    bool enabledInTutorial;

    private void Start()
    {
        if (!enabledInTutorial && Settings.Instance.IsTutorialModeEnabled())
        {
            gameObject.SetActive(false);
        }
    }
}