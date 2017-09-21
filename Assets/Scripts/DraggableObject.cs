// Author(s): Joel Esquilin, Paul Calande

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableObject : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public RectTransform CanvasTransform;
    public Action<DraggableObject> BeginDragEvent;
    public Action<DraggableObject> EndDragEvent;

    public List<SnapLocation> snapToAreas; // Change to list of blocks with position to include other properties 
    public Vector2 defaultPosition;
    public static Vector2 PiecePlacementOffset = new Vector2(100, 100);

    protected Vector2 _pointerOffset = Vector2.zero; //This class should be inheritable to create other new blocks

    // Cached RectTransform.
    protected RectTransform rectTransform;

    protected virtual void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out _pointerOffset);

        if (BeginDragEvent != null)
        {
            BeginDragEvent(this);
        }
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        Vector2 localPointerPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(CanvasTransform, eventData.position, eventData.pressEventCamera, out localPointerPosition))
        {
            float xPos = localPointerPosition.x - _pointerOffset.x;
            float yPos = localPointerPosition.y - _pointerOffset.y;

            transform.localPosition = new Vector2(xPos, yPos);
        }
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        Vector2 position = transform.localPosition;
        bool validSpotFound = false;
        //Vector2 blockToGoTo = Vector2.zero;
        SnapLocation locationToGoTo = null;

        foreach (var solution in snapToAreas)
        {
            float solutionX = solution.transform.position.x;
            float solutionY = solution.transform.position.y;
            if (((position.x > solutionX - PiecePlacementOffset.x) && (position.x < solutionX + PiecePlacementOffset.x))
                && ((position.y > solutionY - PiecePlacementOffset.y) && (position.y < solutionY + PiecePlacementOffset.y)))
            {
                validSpotFound = true;
            }

            if (validSpotFound)
            {
                locationToGoTo = solution;
                break;
            }
        }

        if (validSpotFound)
        {
            //Do something here when at tile
            transform.localPosition = locationToGoTo.transform.position;
            locationToGoTo.Snap(gameObject);
        }
        else
        {
            transform.localPosition = defaultPosition;
        }

        if (EndDragEvent != null)
        {
            EndDragEvent(this);
        }
    }

    protected virtual void OnDestroy()
    {
        BeginDragEvent = null;
        EndDragEvent = null;
    }

    public void SetSnapToAreas(List<SnapLocation> snapLocations)
    {
        snapToAreas = snapLocations;
    }
}