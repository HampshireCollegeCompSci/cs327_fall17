using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScreenTapping : MonoBehaviour {
    [SerializeField]
    [Tooltip("The prefab to instantiate for ScreenTapping.")]
    GameObject prefabScreenTapping;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < Input.touchCount; ++i)
        {
            if (Input.GetTouch(i).phase == TouchPhase.Began)
            {
                GameObject TappingAnim = Instantiate(prefabScreenTapping, transform, false);
                TappingAnim.transform.localPosition = Input.GetTouch(i).position;
            }
        }
    }
}
