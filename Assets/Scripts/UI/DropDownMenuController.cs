using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class DropDownMenuController : MonoBehaviour {

    public Dropdown menu;
    public UIGameOver gameover;
    public UITitleMenus titlemenus;

    UnityAction<int> onValue;

    // Use this for initialization
    void Start () {
        onValue += MenuSelected;
        menu.onValueChanged.AddListener(onValue);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    
    public void MenuSelected(int value)
    {
        if (menu.value == 0)
        {
            gameover.Reset();
        }
        else if (menu.value == 1)
        {
            titlemenus.GoToTitle();
        }
        else if (menu.value == 2)
        {
            //Open settings
        }
    }
}
