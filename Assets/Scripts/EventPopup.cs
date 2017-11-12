using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventPopup : MonoBehaviour {

    [SerializeField]
    Text eventText;

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

    public IEnumerator Translation(string eText, GameObject _canvas, float secondsToStay)
    {
        canvas = _canvas;
        Rect canvasRect = canvas.GetComponent<RectTransform>().rect;

        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(canvasRect.width, canvasRect.height);
        eventText.text = eText;
        eventText.GetComponent<RectTransform>().sizeDelta = new Vector2(canvasRect.width * 8 / 9, canvasRect.height / 8);

        float rotationZ = -16.875f / Camera.main.aspect;
        eventText.transform.Rotate(new Vector3(0, 0, rotationZ));
        Vector3 centerPos = transform.localPosition;
        transform.localPosition = new Vector3(centerPos.x, centerPos.y + canvasRect.height, centerPos.z);
        isTranslating = true;
        yield return new WaitForSeconds(0.5f);
        isTranslating = false;
        yield return new WaitForSeconds(secondsToStay);
        isTranslating = true;
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
