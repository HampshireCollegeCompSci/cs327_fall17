// Author(s): Maia Doerner, Paul Calande

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenTappingFader : MonoBehaviour
{
    [SerializeField]
    [Tooltip("How many seconds it takes for the particle to vanish.")]
    float fadeTime = 0.8f;
    [SerializeField]
    [Tooltip("How quickly the particle moves.")]
    float movementSpeed = 3.0f;
    [SerializeField]
    [Tooltip("How many degrees the particle rotates per second.")]
    float rotationSpeed = 180.0f;
    [SerializeField]
    [Tooltip("Reference to the Image component.")]
    Image image;

    Color imageColor;
    float randomRotate;
    float randomPosition;
    Vector3 direction;
    float elapsedTime = 0.0f;

    void Awake()
    {
        imageColor = image.color;
        randomRotate = Random.Range(0.0f, 360.0f);
        transform.Rotate(0.0f, 0.0f, randomRotate);
        float x = Random.Range(-1.0f, 1.0f);
        float y = Random.Range(-1.0f, 1.0f);
        direction = new Vector3(x, y, 0.0f);
    }

    void Update()
    {
        if (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            imageColor.a = 1.0f - Mathf.Clamp01(elapsedTime / fadeTime);
            image.color = imageColor;
            transform.Rotate(0, 0, Time.deltaTime * rotationSpeed);
            transform.position += direction * movementSpeed;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}