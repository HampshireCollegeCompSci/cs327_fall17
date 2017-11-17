// Author(s): Wm. Josiah Erikson, Paul Calande
// This very simple script is for holding persistent cross-scene settings.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Are cheats enabled? For debug viewing purposes only.")]
    bool cheatsEnabled = false;
    [SerializeField]
    [Tooltip("Whether Zen Mode is enabled or not.")]
    bool zenModeEnabled = false;
    [SerializeField]
    [Tooltip("Whether Tutorial Mode is enabled or not.")]
    bool tutorialModeEnabled = true;

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

        if (GetTutorialComplete())
        {
            SetTutorialModeEnabled(false);
        }
        else
        {
            SetTutorialModeEnabled(true);
        }
    }

    public bool AreCheatsEnabled() //Tell us whether cheats are enabled or not
    {
        //Debug.Log("Cheats are enabled: " + cheatsEnabled);
        return cheatsEnabled;
    }

    public void SetCheatsEnabled() //Set cheats enabled
    {
        cheatsEnabled = true;
        //Debug.Log("Cheats are enabled: " + cheatsEnabled);
    }

    public void SetZenModeEnabled(bool on)
    {
        zenModeEnabled = on;
    }

    public bool IsZenModeEnabled()
    {
        return zenModeEnabled;
    }

    public void SetTutorialModeEnabled(bool on)
    {
        tutorialModeEnabled = on;
    }

    public bool IsTutorialModeEnabled()
    {
        return tutorialModeEnabled;
    }

    string GetHighScoreKeyName()
    {
        if (IsZenModeEnabled())
        {
            return "HighScoreZen";
        }
        else
        {
            return "HighScore";
        }
    }

    // Get the stored high score - 0 if doesn't exist.
    public int GetHighScore()
    {
        return PlayerPrefs.GetInt(GetHighScoreKeyName());
    }

    // Saves the high score.
    public void SaveHighScore(int newHighScore)
    {
        if (!AreCheatsEnabled() && !IsTutorialModeEnabled())
        {
            PlayerPrefs.SetInt(GetHighScoreKeyName(), newHighScore);
        }
    }

    public void SetTutorialComplete()
    {
        PlayerPrefs.SetInt("TutorialDone", 1);
        SetTutorialModeEnabled(false);
    }

    public bool GetTutorialComplete()
    {
        int result = PlayerPrefs.GetInt("TutorialDone");
        return (result == 1);
    }
}