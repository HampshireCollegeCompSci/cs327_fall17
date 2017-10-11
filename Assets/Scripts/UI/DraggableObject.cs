// Author(s): Joel Esquilin, Paul Calande

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// This class should be inheritable to create other new blocks.
public class DraggableObject : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField]
    [Tooltip("Whether the GameObject is currently draggable.")]
    protected bool isDraggable;
    [SerializeField]
    [Tooltip("A List of locations that this GameObject can snap to.")]
    protected List<SnapLocation> snapToAreas;
    [SerializeField]
    [Tooltip("Reference to the UI canvas' RectTransform.")]
    protected RectTransform canvasTransform;
    [SerializeField]
    [Tooltip("Reference to the draggable GameObject's RectTransform.")]
    protected RectTransform rectTransform;
    [SerializeField]
    [Tooltip("The position to return to if dragging ceases and no SnapLocation is snapped to.")]
    protected Vector2 defaultPosition;

    protected static Vector2 piecePlacementOffset = new Vector2(60, 60);

    public Action<DraggableObject> BeginDragEvent;
    public Action<DraggableObject> EndDragEvent;

    protected Vector2 _pointerOffset = Vector2.zero;
    // Whether the object is currently being dragged or not.
    protected bool isDragging = false;

    // Snap detection offset for detecting snap locations.
    Vector2 snapDetectionOffset;

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (isDraggable)
        {
            isDragging = true;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out _pointerOffset);

            if (BeginDragEvent != null)
            {
                BeginDragEvent(this);
            }
        }
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (isDraggable)
        {
            Vector2 localPointerPosition;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasTransform, eventData.position, eventData.pressEventCamera, out localPointerPosition))
            {
                float xPos = localPointerPosition.x - _pointerOffset.x;
                float yPos = localPointerPosition.y - _pointerOffset.y;

                transform.localPosition = new Vector2(xPos, yPos);
            }

            bool validSpotFound = false;
            SnapLocation locationToGoTo = null;

            Vector2 myPos = transform.position;
            myPos += snapDetectionOffset;

            // Used to figure out which location is the closest before snapping.
            float smallestDistance = Mathf.Infinity;

            foreach (SnapLocation solution in snapToAreas)
            {
                Vector2 solutionPos = solution.transform.position;
                float solutionX = solutionPos.x;
                float solutionY = solutionPos.y;

                //Debug.Log("Me: (" + myPos.x + ", " + myPos.y + "), solution: (" + solutionX + ", " + solutionY + ")");

                if ((myPos.x > solutionX - piecePlacementOffset.x) &&
                    (myPos.x < solutionX + piecePlacementOffset.x) &&
                    (myPos.y > solutionY - piecePlacementOffset.y) &&
                    (myPos.y < solutionY + piecePlacementOffset.y))
                {
                    validSpotFound = true;
                }

                if (validSpotFound)
                {
                    float distance = Vector2.Distance(myPos, solutionPos);
                    if (distance < smallestDistance)
                    {
                        locationToGoTo = solution;
                        smallestDistance = distance;
                    }
                }
            }

            if (validSpotFound)
            {
                locationToGoTo.Hover(gameObject, false); // Clear all highlights
                locationToGoTo.Hover(gameObject, true); // Set on highlight for current tile
            }
            else
            {
                snapToAreas[0].Hover(gameObject, false); // Turn off highlights
            }
        }
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if (isDraggable)
        {
            isDragging = false;
            bool validSpotFound = false;
            SnapLocation locationToGoTo = null;

            Vector2 myPos = transform.position;
            myPos += snapDetectionOffset;

            // Used to figure out which location is the closest before snapping.
            float smallestDistance = Mathf.Infinity;

            foreach (SnapLocation solution in snapToAreas)
            {
                Vector2 solutionPos = solution.transform.position;
                float solutionX = solutionPos.x;
                float solutionY = solutionPos.y;

                //Debug.Log("Me: (" + myPos.x + ", " + myPos.y + "), solution: (" + solutionX + ", " + solutionY + ")");

                if ((myPos.x > solutionX - piecePlacementOffset.x) &&
                    (myPos.x < solutionX + piecePlacementOffset.x) &&
                    (myPos.y > solutionY - piecePlacementOffset.y) &&
                    (myPos.y < solutionY + piecePlacementOffset.y))
                {
                    validSpotFound = true;
                }

                if (validSpotFound)
                {
                    float distance = Vector2.Distance(myPos, solutionPos);
                    if (distance < smallestDistance)
                    {
                        locationToGoTo = solution;
                        smallestDistance = distance;
                    }
                }
            }

            if (validSpotFound)
            {
                transform.position = locationToGoTo.transform.position;
                locationToGoTo.Snap(gameObject);
                locationToGoTo.Hover(gameObject, false);
            }
            else
            {
                snapToAreas[0].Hover(gameObject, false); // Turn off highlights
                transform.localPosition = defaultPosition;
            }

            if (EndDragEvent != null)
            {
                EndDragEvent(this);
            }
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

    public void SetIsDraggable(bool draggable)
    {
        isDraggable = draggable;
    }

    public void SetCanvasTransform(RectTransform newCanvasTransform)
    {
        canvasTransform = newCanvasTransform;
    }

    public void SetDefaultPosition(Vector2 position)
    {
        defaultPosition = position;
    }

    public bool GetIsDragging()
    {
        return isDragging;
    }

    public void SetSnapDetectionOffset(Vector2 offset)
    {
        snapDetectionOffset = offset;
    }
}