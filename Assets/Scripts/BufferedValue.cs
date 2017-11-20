// Author(s): Paul Calande
// Handy script for making the display of a value increase and decrease gradually.
// This is generally more polished than simply having a value appear to change instantly.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BufferedValue : MonoBehaviour
{
    enum BufferTargetUpdatePauseConditions
    {
        // Perform buffer pausing under any condition.
        None,

        // The buffer value target update pause will only occur if the difference
        // between the buffer value and its target are zero.
        ZeroBufferDifference,

        // The buffer value target update pause will begin if the difference between the
        // buffer value and its target are zero. The timer will then get reset every time
        // SetBufferedValueUpdate is called until the timer runs out.
        ZeroBufferDifferenceWithTimerReset
    }

    // Invoked at the end of every step.
    // Add callbacks to this to update Text components, etc.
    // -=-=- ARGUMENTS -=-=-
    // 1. newValue: The current value of the buffered value.
    // 2. difference: The difference between the buffered value target and the buffer value.
    // Calculated as bufferValueTarget - bufferedValue.
    public delegate void ValueUpdatedHandler(int newValue, int difference);
    public event ValueUpdatedHandler ValueUpdated;

    [SerializeField]
    [Tooltip("Seconds between each run of the UpdateBufferValue loop.")]
    float secondsBetweenSteps = 0.1f;
    [SerializeField]
    [Tooltip("The size of the steps that the buffered value takes to reach the target.")]
    float stepSize = 0.2f;
    [SerializeField]
    [Tooltip("How many seconds to pause the stepping process for when SetBufferedValueTarget is called.")]
    float secondsToWaitAtTargetUpdate = 0.0f;
    [SerializeField]
    [Tooltip("Determines the conditions for pausing the stepping process for when SetBufferedValueTarget is called.")]
    BufferTargetUpdatePauseConditions bufferTargetUpdatePauseConditions = BufferTargetUpdatePauseConditions.None;

    // The actual quantity that takes steps to catch up to the buffered energy target.
    int bufferedValue = 0;
    // The value that the buffered value is trying to reach.
    int bufferedValueTarget = 0;
    // The number of seconds left when waiting at a target update.
    float secondsLeftAtTargetUpdate = 0.0f;
    // The number of seconds left before the next step.
    float secondsLeftUntilNextStep = 0.0f;

    // Use this to immediately set the buffered value and its target, skipping all the steps.
    // This can be used to initialize the buffered value.
    public void ForceSetBufferedValue(int newValue)
    {
        bufferedValue = bufferedValueTarget = newValue;
        OnValueUpdated(bufferedValue, 0);
    }

    // Set a new target for the buffered value.
    public void SetBufferedValueTarget(int newTarget)
    {
        bufferedValueTarget = newTarget;

        // See the BufferTargetUpdatePauseConditions for a description of each condition.
        // Or don't. The code is rather self-explanatory.
        switch (bufferTargetUpdatePauseConditions)
        {
            case BufferTargetUpdatePauseConditions.None:
                secondsLeftAtTargetUpdate = secondsToWaitAtTargetUpdate;
                break;

            case BufferTargetUpdatePauseConditions.ZeroBufferDifference:
                if (bufferedValue == bufferedValueTarget)
                {
                    secondsLeftAtTargetUpdate = secondsToWaitAtTargetUpdate;
                }
                break;

            case BufferTargetUpdatePauseConditions.ZeroBufferDifferenceWithTimerReset:
                if (secondsLeftAtTargetUpdate > 0.0f)
                {
                    secondsLeftAtTargetUpdate = secondsToWaitAtTargetUpdate;
                }
                else
                {
                    if (bufferedValue == bufferedValueTarget)
                    {
                        secondsLeftAtTargetUpdate = secondsToWaitAtTargetUpdate;
                    }
                }
                break;
        }
    }

    public void AddToBufferedValueTarget(int amount)
    {
        SetBufferedValueTarget(bufferedValueTarget + amount);
    }

    public void SubtractFromBufferedValueTarget(int amount)
    {
        SetBufferedValueTarget(bufferedValueTarget - amount);
    }

    // Keep track of time and perform steps.
    private void Update()
    {
        if (secondsLeftAtTargetUpdate > 0.0f)
        {
            secondsLeftAtTargetUpdate -= Time.deltaTime;
        }
        else
        {
            if (secondsLeftUntilNextStep > 0.0f)
            {
                secondsLeftUntilNextStep -= Time.deltaTime;
            }
            else
            {
                int difference = bufferedValueTarget - bufferedValue;

                int signToCatchUpWith = (bufferedValue < bufferedValueTarget) ? 1 : -1;
                int step = Mathf.CeilToInt(Mathf.Abs(difference) * stepSize) * signToCatchUpWith;
                bufferedValue += step;

                OnValueUpdated(bufferedValue, difference);
                secondsLeftUntilNextStep += secondsBetweenSteps;
            }
        }
    }

    private void OnValueUpdated(int newValue, int difference)
    {
        if (ValueUpdated != null)
        {
            ValueUpdated(newValue, difference);
        }
    }
}