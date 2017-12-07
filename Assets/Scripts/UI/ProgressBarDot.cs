// Author(s): Paul Calande
// Script for a dot on the progress bar.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarDot : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Reference to the dot image.")]
    Image imageDot;
    [SerializeField]
    [Tooltip("Reference to the event icon image.")]
    Image imageEventIcon;
    [SerializeField]
    [Tooltip("Reference to the event icon transform.")]
    RectTransform rectTransformEventIcon;
    [SerializeField]
    [Tooltip("Sprite for the dot if the player didn't reach the event.")]
    Sprite spriteDotOff;
    [SerializeField]
    [Tooltip("Sprite for the dot if the player did reach the event.")]
    Sprite spriteDotOn;
    [SerializeField]
    [Tooltip("Icons in ascending order of tiers for Junkyard.")]
    Sprite[] spriteJunkyard;
    [SerializeField]
    [Tooltip("Icons in ascending order of tiers for Asteroids.")]
    Sprite[] spriteAsteroids;
    [SerializeField]
    [Tooltip("Icons in ascending order of tiers for Radiation.")]
    Sprite[] spriteRadiation;
    [SerializeField]
    [Tooltip("Icon for Reactor Meltdown.")]
    Sprite spriteMeltdown;
    [SerializeField]
    [Tooltip("Icon for Reactor Overload.")]
    Sprite spriteOverload;

    public void SetEvent(VoidEventGroup veg, int score)
    {
        if (score < veg.GetEventGroupBegin())
        {
            imageDot.sprite = spriteDotOff;
        }
        else
        {
            imageDot.sprite = spriteDotOn;
        }

        int arrayIndex = veg.GetEventGroupTier() - 1;
        switch (veg.GetEventGroupType())
        {
            case VoidEventGroup.EventGroupType.Junkyard:
                imageEventIcon.sprite = spriteJunkyard[arrayIndex];
                break;
            case VoidEventGroup.EventGroupType.Asteroids:
                imageEventIcon.sprite = spriteAsteroids[arrayIndex];
                break;
            case VoidEventGroup.EventGroupType.Radiation:
                imageEventIcon.sprite = spriteRadiation[arrayIndex];
                break;
            case VoidEventGroup.EventGroupType.Meltdown:
                imageEventIcon.sprite = spriteMeltdown;
                break;
            case VoidEventGroup.EventGroupType.Overload:
                imageEventIcon.sprite = spriteOverload;
                break;
        }
    }

    // Move the event icon vertically to the other side of the dot.
    public void MoveIcon()
    {
        Vector2 anchorPos = rectTransformEventIcon.anchoredPosition;
        rectTransformEventIcon.anchoredPosition = -anchorPos;
    }
}