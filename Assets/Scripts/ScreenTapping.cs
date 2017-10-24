// Author(s): Yixiang Xu
/* This class is working for screen tapping
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class ScreenTapping : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    [Tooltip("The prefab to instantiate for ScreenTapping.")]
    GameObject prefabScreenTapping;

    Vector2 localPoint;

    public void OnBeginDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out localPoint);
        GameObject tappingAnim = Instantiate(prefabScreenTapping, transform, false);
        tappingAnim.transform.localPosition = localPoint;
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out localPoint);
        GameObject tappingAnim = Instantiate(prefabScreenTapping, transform, false);
        tappingAnim.transform.localPosition = localPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out localPoint);
        GameObject tappingAnim = Instantiate(prefabScreenTapping, transform, false);
        tappingAnim.transform.localPosition = localPoint;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out localPoint);
        GameObject tappingAnim = Instantiate(prefabScreenTapping, transform, false);
        tappingAnim.transform.localPosition = localPoint;
    }
}
