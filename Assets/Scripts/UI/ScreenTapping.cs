// Author(s): Yixiang Xu, Paul Calande
// This script is responsible for creating the particle effect for the pointer.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class ScreenTapping : MonoBehaviour//, IDragHandler, IPointerDownHandler
{
    [SerializeField]
    [Tooltip("The prefab to instantiate for ScreenTapping.")]
    GameObject prefabScreenTapping;
    [SerializeField]
    [Tooltip("Reference to the canvas' RectTransform component.")]
    RectTransform canvas;

    /*
    public void OnDrag(PointerEventData eventData)
    {
        TappingEffect(eventData.position);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        TappingEffect(eventData.position);
    }
    */

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 pos = Input.mousePosition;
            TappingEffect(pos);
            //Debug.Log(pos);
        }
    }

    //public void TappingEffect(PointerEventData eventData)
    public void TappingEffect(Vector2 position)
    {
        //Vector2 localPoint;
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out localPoint);
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, position, Camera.main, out localPoint);
        //GameObject tappingAnim = Instantiate(prefabScreenTapping, transform, false);
        //tappingAnim.transform.localPosition = localPoint;

        //GameObject tappingAnim = Instantiate(prefabScreenTapping, position, Quaternion.identity);

        Vector2 localPoint = canvas.InverseTransformPoint(position);
        GameObject tappingAnim = Instantiate(prefabScreenTapping, transform, false);
        tappingAnim.transform.localPosition = localPoint;
    }
}