//Author: Wm. Josiah Erikson
//This very simple script is for holding persistent cross-scene settings
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour {
    [SerializeField]
    [Tooltip("Are cheats enabled? For debug viewing purposes only.")]
    bool cheatsEnabled; //Are there cheats enabled or not?

    private static Settings instance = null;
    public static Settings Instance
    {
        get
        {
            return instance;
        }
    }


    void Awake()
    {
        if (instance)
        {
            DestroyImmediate(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }


    public bool AreCheatsEnabled () //Tell us whether cheats are enabled or not
    {
        //Debug.Log("Cheats are enabled: " + cheatsEnabled);
        return cheatsEnabled;
    }

    public void SetCheatsEnabled () //Set cheats enabled
    {
        cheatsEnabled = true;
        //Debug.Log("Cheats are enabled: " + cheatsEnabled);
    }
}
