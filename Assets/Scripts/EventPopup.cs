// Author(s): Yifeng Shi, Paul Calande
// Script that controls an event started popup.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventPopup : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Text for the title of the event.")]
    Text eventText;
    [SerializeField]
    [Tooltip("Text for the description of the event.")]
    Text descriptionText;
    [SerializeField]
    [Tooltip("Reference to the image to use for the panel.")]
    Image panel;
    [SerializeField]
    [Tooltip("Tier-based sprites for the panel, from lowest tier to highest tier.")]
    Sprite[] spriteTiers;

    bool isTranslating;
    GameObject canvas;

	public Text GetEventText()
    {
        return eventText;
    }

    private void Update()
    {
        if (isTranslating)
        {
            gameObject.transform.Translate(Vector3.down * canvas.GetComponent<RectTransform>().rect.height * Time.deltaTime * Screen.height / 1000);
        }
    }

    public void Init(string eText, string dText, GameObject _canvas, int tier, float secondsToStay)
    {
        panel.sprite = spriteTiers[tier - 1];

        eventText.text = eText;
        descriptionText.text = dText;

        canvas = _canvas;
        Rect canvasRect = canvas.GetComponent<RectTransform>().rect;

        //gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(canvasRect.width, canvasRect.height);
        //eventText.GetComponent<RectTransform>().sizeDelta = new Vector2(canvasRect.width * 8 / 9, canvasRect.height / 8);

        /*
        float rotationZ = -16.875f / Camera.main.aspect;
        eventText.transform.Rotate(new Vector3(0, 0, rotationZ));
        */

        Vector3 centerPos = transform.localPosition;
        transform.localPosition = new Vector3(centerPos.x, centerPos.y + canvasRect.height, centerPos.z);

        StartCoroutine(Translation(secondsToStay));
    }

    public IEnumerator Translation(float secondsToStay)
    {
        isTranslating = true;
        yield return new WaitForSeconds(0.5f);
        isTranslating = false;
        yield return new WaitForSeconds(secondsToStay);
        isTranslating = true;
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}