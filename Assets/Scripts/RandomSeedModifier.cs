//Author(s): Someone, Wm. Josiah Erikson
//Sets random seed to 0 for purpose of testing
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomSeedModifier : MonoBehaviour {
   
    Settings settings;
    private void Awake()
    {
        settings = FindObjectOfType<Settings>(); //Can't pass in a reference because it's a singleton
    }

    public void SetRandomSeedToZero() {
		Random.InitState (0);
        settings.SetCheatsEnabled(); //Tell the game that cheats have been enabled
	}

}
