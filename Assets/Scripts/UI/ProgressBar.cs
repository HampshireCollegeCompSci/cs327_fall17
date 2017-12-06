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
    [SerializeField]
    [Tooltip("y anchor for progress bar dots.")]
    float dotAnchorY;

    List<VoidEventGroup> voidEventGroups;

    // Populate the progress bar with events.
    public void InstantiateEvents(List<VoidEventGroup> voidEventGroupsIn, int score,
        VoidEventController vec)
    {
        voidEventGroups = voidEventGroupsIn;
        foreach (VoidEventGroup veg in voidEventGroups)
        {
            // Instantiate prefab instance.
            GameObject newObj = Instantiate(prefabProgressBarDot, transform, false);
            // Get component and pass the VoidEventGroup.
            newObj.GetComponent<ProgressBarDot>().SetEvent(veg, score);
            // Move the object to the appropriate place on the progress meter.
            RectTransform rt = newObj.GetComponent<RectTransform>();
            float progress = vec.GetProgress(veg.GetEventGroupBegin());
            rt.anchorMin = new Vector2(progress, dotAnchorY);
            rt.anchorMax = new Vector2(progress, dotAnchorY);
        }
    }

    /*
    // Set the player's progress on the progress bar.
    public void SetProgress(float percent)
    {
        //progressBarTop.anchorMax = new Vector2(progress, progressBarTop.anchorMax.y);
        //Debug.Log(progressBarTop.anchorMax);
    }
    */
}