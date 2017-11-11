using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventPopup : MonoBehaviour {

    [SerializeField]
    Text eventText;

	public Text GetEventText()
    {
        return eventText;
    }
}
