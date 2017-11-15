// Author(s): Paul Calande
// Script for the event slider object on the reactor.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSlider : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Reference to the slider's RectTransform.")]
    RectTransform rectTransform;
    [SerializeField]
    [Tooltip("How quickly the event slider can move.")]
    float moveSpeed = 0.2f;

    //State targetState = State.None;

    float yBottom;
    float yTop;

    float targetX;
    float currentX;

    private void Awake()
    {
        yBottom = rectTransform.anchorMin.y;
        yTop = rectTransform.anchorMax.y;
        SetCurrentState(VoidEventGroup.EventGroupType.None);
        currentX = targetX;
        SetCenterX(currentX);
    }

    private void Update()
    {
        if (currentX != targetX)
        {
            float difference = targetX - currentX;
            int sign = (difference > 0) ? 1 : -1;
            float speedThisFrame = moveSpeed * Time.deltaTime;
            currentX += speedThisFrame * sign;
            if (Mathf.Abs(difference) < speedThisFrame)
            {
                currentX = targetX;
            }
            SetCenterX(currentX);
        }
    }

    public void SetCurrentState(VoidEventGroup.EventGroupType newState)
    {
        //targetState = newState;
        switch (newState)
        {
            case VoidEventGroup.EventGroupType.None:
                SetTargetX(0.1f);
                break;

            case VoidEventGroup.EventGroupType.Asteroids:
                SetTargetX(0.3f);
                break;

            case VoidEventGroup.EventGroupType.Junkyard:
                SetTargetX(0.5f);
                break;

            case VoidEventGroup.EventGroupType.Radiation:
                SetTargetX(0.72f);
                break;

            case VoidEventGroup.EventGroupType.Meltdown:
            case VoidEventGroup.EventGroupType.Overload:
                SetTargetX(0.9f);
                break;
        }
    }

    private void SetCenterX(float centerX)
    {
        float radius = 0.02f;
        rectTransform.anchorMin = new Vector2(centerX - radius, yBottom);
        rectTransform.anchorMax = new Vector2(centerX + radius, yTop);
    }

    private void SetTargetX(float newTargetX)
    {
        targetX = newTargetX;
    }
}