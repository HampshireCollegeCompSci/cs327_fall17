// Author(s): Paul Calande
// Rising text that accumulates next to the reactor.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RisingTextAccumulator : MonoBehaviour
{
    // Invoked when LerpTo is Completed.
    public delegate void CompletedHandler(int amount);
    public event CompletedHandler Completed;
    // Invoked when starting to move.
    public delegate void MoveStartedHandler();
    public event MoveStartedHandler MoveStarted;

    [SerializeField]
    [Tooltip("Number of seconds to wait before heading towards reactor.")]
    float secondsToWait;
    [SerializeField]
    [Tooltip("Number of seconds to take when heading into reactor.")]
    float secondsToTravel;
    [SerializeField]
    [Tooltip("Reference to the reactor center. This is where the accumulated text heads to.")]
    Transform reactorCenter;
    [SerializeField]
    [Tooltip("Reference to the LerpTo component.")]
    LerpTo lerpTo;
    [SerializeField]
    [Tooltip("Reference to the BufferedValue component.")]
    BufferedValue bufferedValue;
    [SerializeField]
    [Tooltip("Reference to the Text component.")]
    Text text;

    // How many seconds the accumulator has waited prior to moving.
    float secondsWaited = 0.0f;

    private bool movingYet = false;

    int quantity = 0;

    private void Awake()
    {
        lerpTo.Completed += LerpTo_Completed;
    }

    private void Start()
    {
        bufferedValue.ForceSetBufferedValue(quantity);
    }

    private void Update()
    {
        if (!movingYet)
        {
            secondsWaited += Time.deltaTime;
            if (secondsWaited > secondsToWait)
            {
                lerpTo.LerpToInTime(reactorCenter.position, secondsToTravel);
                movingYet = true;
                OnMoveStarted();
            }
        }
    }

    public void Init()
    {
        bufferedValue.ForceSetBufferedValue(0);
    }

    public void SetReactorCenter(Transform trans)
    {
        reactorCenter = trans;
    }

    public void SetColor(Color col)
    {
        text.color = col;
    }

    public void SetSecondsToWait(float sec)
    {
        secondsToWait = sec;
    }

    public void SetSecondsToTravel(float sec)
    {
        secondsToTravel = sec;
    }

    public void ResetTimer()
    {
        secondsWaited = 0.0f;
    }

    public void Add(int amount)
    {
        quantity += amount;
        bufferedValue.AddToBufferedValueTarget(amount);
    }

    private void OnCompleted()
    {
        if (Completed != null)
        {
            Completed(quantity);
            Destroy(gameObject);
        }
    }

    private void OnMoveStarted()
    {
        if (MoveStarted != null)
        {
            MoveStarted();
        }
    }

    private void LerpTo_Completed()
    {
        OnCompleted();
    }
}