// Author(s): Paul Calande, Wm. Josiah Erikson

// Comment out the following line to use transform.position instead of transform.localPosition.
// transform.localPosition might not function properly.
//#define USING_LOCAL_POSITION

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class allows us to easily move game objects around
public class LerpTo : MonoBehaviour
{
    public delegate void CompletedHandler();
    public event CompletedHandler Completed;

    enum LerpState
    {
        None,
        Speed,
        Time
    }

    LerpState state = LerpState.None;

    private float movingSpeed;
    private float movingTime;
    private Vector3 movingDestination;
    private Vector3 startPosition;
    private float startTime;
    private float journeyLength;
    private float t; //to keep track of how far we've gone towards our goal if we're timed

    // When this method is called, the GameObject moves towards destination 
    // at the given speed and will keep moving until it reaches the destination.
    public void LerpToAtSpeed(Vector3 destination, float speed)
    {
        MovingSetup();
        state = LerpState.Speed;
        movingSpeed = speed;
        SetMovingDestination(destination);
    }

    // When this method is called, the GameObject moves towards the destination
    // and reaches it in the given amount of time (in seconds).
    public void LerpToInTime(Vector3 destination, float time)
    {
        MovingSetup();
        state = LerpState.Time;
        movingTime = time;
        t = 0.0f;
        SetMovingDestination(destination);
    }

    private void SetMovingDestination(Vector3 newDest)
    {
#if USING_LOCAL_POSITION
        movingDestination = transform.InverseTransformPoint(newDest);
#else
        movingDestination = newDest;
#endif
    }

    // This method sets all of the variables up to start moving.
    private void MovingSetup()
    {
        startTime = Time.time;

#if USING_LOCAL_POSITION
        startPosition = transform.localPosition;
#else
        startPosition = transform.position;
#endif

        journeyLength = Vector3.Distance(gameObject.transform.position, movingDestination);
    }

    private void Update()
    {
        if (state != LerpState.None)
        {
            float progressPercent = 0.0f;
            Vector3 newPos = Vector3.zero;
            switch (state)
            {
                case LerpState.Speed:
                    float distCovered = (Time.time - startTime) * movingSpeed;
                    float fracJourney = distCovered / journeyLength;
                    newPos = Vector3.Lerp(startPosition, movingDestination, fracJourney);
                    progressPercent = fracJourney;
                    break;

                case LerpState.Time:
                    t += Time.deltaTime / movingTime;
                    newPos = Vector3.Lerp(startPosition, movingDestination, t);
                    progressPercent = t;
                    break;
            }
#if USING_LOCAL_POSITION
            transform.localPosition = newPos;
#else
            transform.position = newPos;
#endif
            // If we've reached our destination, we're done.
            //if (movingDestination == newPos)
            if (progressPercent >= 1.0f)
            {
                state = LerpState.None;
                OnCompleted();
            }
        }
    }

    // Invoke the Completed event.
    void OnCompleted()
    {
        if (Completed != null)
        {
            Completed();
        }
    }
}