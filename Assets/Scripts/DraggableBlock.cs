// Author(s): Paul Calande, Joel Esquilin

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableBlock : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    Block block;
    public RectTransform CanvasTransform;
    public Action<DraggableBlock> BeginDragEvent;
    public Action<DraggableBlock> EndDragEvent;

    public List<Vector2> snapToAreas; // Change to list of blocks with position to include other properties 
    public Vector2 defaultPosition;
    public static Vector2 PiecePlacementOffset = new Vector2(100, 100);
    
    protected Vector2 _pointerOffset = Vector2.zero; //This class should be inheritable to create other new blocks

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out _pointerOffset);

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
        Vector2 blockToGoTo = Vector2.zero;

        foreach (var solution in snapToAreas)
        {
            if (((position.x > solution.x - PiecePlacementOffset.x) && (position.x < solution.x + PiecePlacementOffset.x))
                && ((position.y > solution.y - PiecePlacementOffset.y) && (position.y < solution.y + PiecePlacementOffset.y)))
            {
                validSpotFound = true;
            }

            if (validSpotFound)
            {
                blockToGoTo = solution;
                break;
            }
        }

        if (validSpotFound)
        {
            //Do something here when at tile
            transform.localPosition = blockToGoTo;
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

    public void SetBlock(Block copiedBlock)
    {

    }

    public Block GetBlock()
    {
        return block;
    }
}