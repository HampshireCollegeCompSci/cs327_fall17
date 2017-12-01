// Author(s): Yixiang Xu
/* This class is working for screen tapping
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class ScreenTapping : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    [SerializeField]
    [Tooltip("The prefab to instantiate for ScreenTapping.")]
    GameObject prefabScreenTapping;

    public void OnDrag(PointerEventData eventData)
    {
        TappingEffect(eventData);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        TappingEffect(eventData);
    }

    public void TappingEffect(PointerEventData eventData)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out localPoint);
        GameObject tappingAnim = Instantiate(prefabScreenTapping, transform, false);
        tappingAnim.transform.localPosition = localPoint;
    }
}
