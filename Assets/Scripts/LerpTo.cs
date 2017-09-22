// Author(s): Paul Calande, Wm. Josiah Erikson

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class allows us to easily move game objects around
public class LerpTo : MonoBehaviour
{
    //These variables tell us if we're moving...
    private bool lerpingAtSpeed;
    private bool lerpingInTime;
    private float movingSpeed;
    private float movingTime;
    private Vector3 movingDestination;
    private Vector3 startPosition;
    private float startTime;
    private float journeyLength;
    private float t; //to keep track of how far we've gone towards our goal if we're timed

    //When this method is called, the GameObject moves towards destination 
    //at the given speed and will keep moving until it reaches the destination.

    public void LerpToAtSpeed(Vector3 destination, float speed)
    {
        lerpingAtSpeed = true;
        movingSpeed = speed;
        movingSetup();
    }

	//When this method is called, the GameObject moves towards the destination
    //and reaches it in the given amount of time (in seconds).
	public void LerpToInTime(Vector3 destination, float time)
    {
        movingSetup();
        lerpingInTime = true;
        movingTime = time;
        t = 0;

    }

    //This method sets all the variables up to start moving
    private void movingSetup() 
    {
		startTime = Time.time;
		startPosition = transform.position;
		journeyLength = Vector3.Distance(gameObject.transform.position, movingDestination);
    }

    private void Update()
    {
        //If we've reached our destination, we're done
        if (movingDestination == transform.position) {
            lerpingInTime = false;
            lerpingAtSpeed = false;
        }
        if (lerpingAtSpeed == true) {
            float distCovered = (Time.time - startTime) * movingSpeed;
            float fracJourney = distCovered / journeyLength;
            transform.position = Vector3.Lerp(startPosition, movingDestination, fracJourney);
        }
        if (lerpingInTime == true) {
            t += Time.deltaTime / movingTime;
            transform.position = Vector3.Lerp(startPosition, movingDestination, t);
        }
    }
}