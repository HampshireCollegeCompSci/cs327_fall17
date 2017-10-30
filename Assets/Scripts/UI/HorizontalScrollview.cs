using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HorizontalScrollview : MonoBehaviour, IEndDragHandler{

    public ScrollRect scrollview;

    public Vector2 currentPos;
    public Vector2 position2;
    public Vector2 position3;

    public float swipeOffset;
    public GameObject slides;

    public Image slide1;
    public Image slide2;
    public Image slide3;

    public void OnEndDrag(PointerEventData eventData)
    {
        float offset = currentPos.x - scrollview.content.localPosition.x;

        if (offset >= currentPos.x + swipeOffset)
        {
            if (currentPos == Vector2.zero)
            {
                NextSlide();
                currentPos = position2;
            } else if (currentPos == position2)
            {
                NextSlide();
                currentPos = position3;
            }
        }else if (offset < swipeOffset)
        {
            if (currentPos == position2)
            {
                PreviousSlide();
                currentPos = Vector2.zero;
            } else if (currentPos == position3)
            {
                PreviousSlide();
                currentPos = position2;
            }
        }
    }

    void NextSlide()
    {
        if (currentPos == position2)
        {
            slides.transform.localPosition = position3;
            slide2.enabled = false;
            slide3.enabled = true;
        }
        else
        {
            slides.transform.localPosition = position2;
            slide1.enabled = false;
            slide2.enabled = true;
        }
    }

    void PreviousSlide()
    {
        if (currentPos == position2)
        {
            slides.transform.localPosition = new Vector2(0f, position2.y);
            slide2.enabled = false;
            slide1.enabled = true;
        }
        else
        {
            slides.transform.localPosition = position2;
            slide3.enabled = false;
            slide2.enabled = true;
        }
    }

}
