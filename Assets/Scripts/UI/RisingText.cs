// Author(s): Paul Calande

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RisingText : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Reference to the Text component to modify.")]
    Text textRising;
    [SerializeField]
    [Tooltip("The speed at which this GameObject rises.")]
    float risingSpeed;
    [SerializeField]
    [Tooltip("How much the risingSpeed decreases per second.")]
    float risingSpeedDecayRate;
    [SerializeField]
    [Tooltip("The speed at which the Text fades into nothingness.")]
    float fadingSpeed;
    /*
    [SerializeField]
    [Tooltip("How many seconds the rising text has to exist before it heads to its destination (if it exists).")]
    float secondsBeforeHeadingToDestination = 1.0f;
    */
    [SerializeField]
    [Tooltip("How many seconds it takes for the rising text to reach its destination once it starts moving.")]
    float secondsToReachDestination;
    [SerializeField]
    [Tooltip("Reference to the LerpTo component of the RisingText.")]
    LerpTo lerpTo;

    // Whether or not the RisingText has a destination set.
    bool hasDestination = false;
    // The location to move to when rising is finished, if not null.
    Vector3 destination;

    // Whether or not the rising text is moving to its destination.
    bool movingToDestination = false;

    // How long the rising text has existed for.
    float secondsElapsed = 0.0f;
    
    /*
    private void Start()
    {
        Debug.Log("Hey YouTube, RisingText here.");
    }
    */

    void Update()
    {
        secondsElapsed += Time.deltaTime;

        float x = textRising.transform.localPosition.x;
        float y = textRising.transform.localPosition.y + risingSpeed * Time.deltaTime;
        float z = textRising.transform.localPosition.z;

        textRising.transform.localPosition = new Vector3(x, y, z);

        if (risingSpeed > 0.0f)
        {
            risingSpeed -= risingSpeedDecayRate * Time.deltaTime;
            if (risingSpeed < 0.0f)
            {
                risingSpeed = 0.0f;
            }
        }

        float r = textRising.color.r;
        float g = textRising.color.g;
        float b = textRising.color.b;
        float a = 1.0f;
        if (!hasDestination)
        {
            float texta = textRising.color.a;
            a = texta - fadingSpeed * Time.deltaTime;
            //Debug.Log("RisingText.Update: a / texta: " + a + " / " + texta);
        }

        textRising.color = new Color(r, g, b, a);

        if (hasDestination)
        {
            if (!movingToDestination)
            {
                //if (secondsElapsed > secondsBeforeHeadingToDestination)
                if (risingSpeed <= 0.0f)
                {
                    movingToDestination = true;
                    lerpTo.LerpToInTime(destination, secondsToReachDestination);
                    lerpTo.Completed += LerpTo_Completed;
                }
            }
        }
        else
        {
            if (a <= 0.0f)
            {
                //Debug.Log("Alpha less than zero.");
                Destroy(gameObject);
            }
        }
    }

    public void SetText(string str)
    {
        textRising.text = str;
    }

    public void SetColor(Color col)
    {
        textRising.color = col;
    }

    public void SetDestination(Vector3 destinationPos)
    {
        destination = destinationPos;
        hasDestination = true;
    }

    /*
    private void OnDestroy()
    {
        Debug.Log("RisingText.OnDestroy");
    }
    */

    void LerpTo_Completed()
    {
        //Debug.Log("RisingText.LerpTo_Completed");
        Destroy(gameObject);
    }
}