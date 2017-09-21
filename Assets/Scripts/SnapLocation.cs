// Author(s): Paul Calande

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapLocation : MonoBehaviour
{
    public delegate void SnappedToHandler(GameObject snapper);
    public event SnappedToHandler SnappedTo;

    public void Snap(GameObject snapper)
    {
        OnSnapped(snapper);
    }

    private void OnSnapped(GameObject snapper)
    {
        if (SnappedTo != null)
        {
            SnappedTo(snapper);
        }
    }
}