// Author(s): Paul Calande

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSlider : MonoBehaviour
{
    public enum State
    {
        None,
        Asteroids,
        Junkyard,
        Radiation,
        //Meltdown,
        Overload
    }

    [SerializeField]
    [Tooltip("Reference to the slider's RectTransform.")]
    RectTransform rectTransform;

    //State targetState = State.None;

    float yBottom;
    float yTop;

    private void Awake()
    {
        yBottom = rectTransform.anchorMin.y;
        yTop = rectTransform.anchorMax.y;
        SetCurrentState(State.None);
    }

    public void SetCurrentState(State newState)
    {
        //targetState = newState;
        switch (newState)
        {
            case State.None:
                SetCenterX(0.1f);
                break;

            case State.Asteroids:
                SetCenterX(0.3f);
                break;

            case State.Junkyard:
                SetCenterX(0.5f);
                break;

            case State.Radiation:
                SetCenterX(0.72f);
                break;

            case State.Overload:
                SetCenterX(0.9f);
                break;
        }
    }

    void SetCenterX(float centerX)
    {
        float radius = 0.02f;
        rectTransform.anchorMin = new Vector2(centerX - radius, yBottom);
        rectTransform.anchorMax = new Vector2(centerX + radius, yTop);
    }
}