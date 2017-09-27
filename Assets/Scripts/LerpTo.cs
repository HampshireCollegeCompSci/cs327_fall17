// Author(s): Paul Calande, Wm. Josiah Erikson

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
        movingDestination = transform.InverseTransformPoint(destination);
        state = LerpState.Speed;
        movingSpeed = speed;
    }

    // When this method is called, the GameObject moves towards the destination
    // and reaches it in the given amount of time (in seconds).
    public void LerpToInTime(Vector3 destination, float time)
    {
        MovingSetup();
        movingDestination = transform.InverseTransformPoint(destination);
        state = LerpState.Time;
        movingTime = time;
        t = 0.0f;
    }

    // This method sets all of the variables up to start moving.
    private void MovingSetup()
    {
        startTime = Time.time;
        startPosition = transform.localPosition;
        journeyLength = Vector3.Distance(gameObject.transform.position, movingDestination);
    }

    private void Update()
    {
        if (state != LerpState.None)
        {
            switch (state)
            {
                case LerpState.Speed:
                    float distCovered = (Time.time - startTime) * movingSpeed;
                    float fracJourney = distCovered / journeyLength;
                    transform.localPosition = Vector3.Lerp(startPosition, movingDestination, fracJourney);
                    break;

                case LerpState.Time:
                    t += Time.deltaTime / movingTime;
                    transform.localPosition = Vector3.Lerp(startPosition, movingDestination, t);
                    break;
            }
            // If we've reached our destination, we're done.
            if (movingDestination == transform.localPosition)
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