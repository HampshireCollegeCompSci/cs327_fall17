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
    public Vector2 position4;
    public Vector2 position5;

    public float swipeOffset;
    public GameObject slides;

    public Image slide1;
    public Image slide2;
    public Image slide3;
    public Image slide4;
    public Image slide5;

    public Image slide1SS;
    public Image slide2SS;
    public Image slide3SS;
    public Image slide4SS;
    public Image slide5SS;

    public Sprite BGLeft;
    public Sprite BGMiddle;
    public Sprite BGRight;
    public Image bg;

    public void OnEndDrag(PointerEventData eventData)
    {
        float offset = currentPos.x - scrollview.content.localPosition.x;

        if (offset >= currentPos.x + swipeOffset)
        {
            if (currentPos == Vector2.zero)
            {
                NextSlide();
            } else if (currentPos == position2)
            {
                NextSlide();
            }
            else if (currentPos == position3)
            {
                NextSlide();
            }
            else if (currentPos == position4)
            {
                NextSlide();
            }
        }
        else if (offset < swipeOffset)
        {
            if (currentPos == position2)
            {
                PreviousSlide();
            } else if (currentPos == position3)
            {
                PreviousSlide();
            }
            else if (currentPos == position4)
            {
                PreviousSlide();
            }
            else if (currentPos == position5)
            {
                PreviousSlide();
            }
        }
    }

    public void NextSlide()
    {
        if (currentPos == position2)
        {
            slides.transform.localPosition = position3;
            slide2.enabled = false;
            slide3.enabled = true;
            slide2SS.enabled = false;
            slide3SS.enabled = true;
            bg.sprite = BGMiddle;
            currentPos = position3;
        }
        else if (currentPos == position3)
        {
            slides.transform.localPosition = position4;
            slide3.enabled = false;
            slide4.enabled = true;
            slide3SS.enabled = false;
            slide4SS.enabled = true;
            bg.sprite = BGMiddle;
            currentPos = position4;
        }
        else if (currentPos == position4)
        {
            slides.transform.localPosition = position5;
            slide4.enabled = false;
            slide5.enabled = true;
            slide4SS.enabled = false;
            slide5SS.enabled = true;
            bg.sprite = BGRight;
            currentPos = position5;
        }
        else if (currentPos == Vector2.zero)
        {
            slides.transform.localPosition = position2;
            slide1.enabled = false;
            slide2.enabled = true;
            slide1SS.enabled = false;
            slide2SS.enabled = true;
            bg.sprite = BGMiddle;
            currentPos = position2;
        }
    }

    public void PreviousSlide()
    {
        if (currentPos == position2)
        {
            slides.transform.localPosition = new Vector2(0f, position2.y);
            slide2.enabled = false;
            slide1.enabled = true;
            slide2SS.enabled = false;
            slide1SS.enabled = true;
            bg.sprite = BGLeft;
            currentPos = Vector2.zero;
        }
        else if (currentPos == position3)
        {
            slides.transform.localPosition = position2;
            slide3.enabled = false;
            slide2.enabled = true;
            slide3SS.enabled = false;
            slide2SS.enabled = true;
            bg.sprite = BGMiddle;
            currentPos = position2;
        }
        else if (currentPos == position4)
        {
            slides.transform.localPosition = position3;
            slide4.enabled = false;
            slide3.enabled = true;
            slide4SS.enabled = false;
            slide3SS.enabled = true;
            bg.sprite = BGMiddle;
            currentPos = position3;
        }
        else if (currentPos == position5)
        {
            slides.transform.localPosition = position4;
            slide5.enabled = false;
            slide4.enabled = true;
            slide5SS.enabled = false;
            slide4SS.enabled = true;
            bg.sprite = BGMiddle;
            currentPos = position4;
        }
    }

}
