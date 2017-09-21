// Author(s): Paul Calande

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableBlock : MonoBehaviour
{
    Block block;
    bool isDraggable = false;

    public void SetBlock(Block copiedBlock)
    {

    }

    public Block GetBlock()
    {
        return block;
    }

    public void AllowDragging()
    {
        isDraggable = true;
    }

    void UpdateAvailableSpaces()
    {

    }

    public void Rotate(bool clockwise)
    {

    }
}