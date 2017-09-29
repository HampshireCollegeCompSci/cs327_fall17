// Author(s): Paul Calande

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapLocation : MonoBehaviour
{
    public delegate void SnappedToHandler(GameObject snapper);
    public event SnappedToHandler SnappedTo;

    public delegate void HoveringHandler(GameObject snapper, bool on);
    public event HoveringHandler HoveringTo;

    public void Snap(GameObject snapper)
    {
        OnSnapped(snapper);
    }

    public void Hover(GameObject snapper, bool on)
    {
        OnHover(snapper, on);
    }

    private void OnSnapped(GameObject snapper)
    {
        if (SnappedTo != null)
        {
            SnappedTo(snapper);
        }
    }

    private void OnHover(GameObject snapper, bool on)
    {
        if (HoveringTo != null)
        {
            HoveringTo(snapper, on);
        }
    }
}