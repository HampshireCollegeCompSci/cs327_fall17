// Author(s): Joel Esquilin, Paul Calande

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIRotate : MonoBehaviour
{
    [SerializeField]
    BlockSpawner spawner;

    public void RotateClockwise()
    {
        spawner.RotateCurrentBlock(true);
    }

    public void RotateCounter()
    {
        spawner.RotateCurrentBlock(false);
    }

    public void Flip()
    {
        spawner.FlipCurrentBlock();
    }
}