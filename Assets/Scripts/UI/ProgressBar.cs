// Author(s): Paul Calande
// The progress bar for the game over screen.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Prefab for the progress bar dot.")]
    GameObject prefabProgressBarDot;

    List<VoidEventGroup> voidEventGroups;

    // Populate the progress bar with events.
    public void InstantiateEvents(List<VoidEventGroup> voidEventGroupsIn)
    {
        voidEventGroups = voidEventGroupsIn;
        foreach (VoidEventGroup veg in voidEventGroups)
        {
            // Instantiate prefab.
            GameObject newObj = Instantiate(prefabProgressBarDot, transform, false);
            // Get component and pass the VoidEventGroup.

        }
    }

    // Set the player's progress on the progress bar.
    public void SetProgress(float percent)
    {
        //progressBarTop.anchorMax = new Vector2(progress, progressBarTop.anchorMax.y);
    }
}