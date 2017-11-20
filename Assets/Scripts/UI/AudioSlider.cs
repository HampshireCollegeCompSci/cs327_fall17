// Author(s): Paul Calande, Joel Esquilin
// Script for a slider that controls audio.
// The slider's range should be [0, 1], meaning 0% to 100% volume.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioSlider : MonoBehaviour, IPointerUpHandler
{
    // Invoked when the mute status changes. The argument is the new mute state.
    public delegate void MuteUpdatedHandler(bool isMuted);
    public event MuteUpdatedHandler MuteUpdated;

    [SerializeField]
    [Tooltip("Reference to the AudioMixer to modify the parameter of.")]
    AudioMixer mixer;
    [SerializeField]
    [Tooltip("The name of the audio mixer's parameter that this slider controls.")]
    string mixerParameter;
    [SerializeField]
    [Tooltip("The slider to use for controlling the audio.")]
    Slider slider;
    [SerializeField]
    [Tooltip("Whether the values (true value and non-muted value) of the slider should be saved.")]
    bool saveValuesToDisk;

    // The last not-muted value that the slider was let go of at.
    float lastNotMutedValue;

    // The minimum amount of decibels that the slider can reach.
    // This value should correspond to the audio being muted.
    static float minimumDecibels = -80.0f;

    private void Start()
    {
        float value;
        mixer.GetFloat(mixerParameter, out value);
        slider.value = DecibelsToPercent(value);

        if (saveValuesToDisk)
        {
            //lastNotMutedValue = DecibelsToPercent(PlayerPrefs.GetFloat(GetFullParameterNameNotMuted(), value));
            lastNotMutedValue = PlayerPrefs.GetFloat(GetFullParameterNameNotMuted(), slider.value);
        }
        else
        {
            lastNotMutedValue = slider.value;
        }
        // If the volume is muted by default, lastNotMuted will be equal to muted volume.
        // In this scenario, set lastNotMuted to maximum volume!
        if (lastNotMutedValue == GetMuteVolume())
        {
            lastNotMutedValue = slider.maxValue;
        }

        // Update the mixer and event subscribers if necessary.
        SliderChangeValue();
    }

    /*
    private static float PercentToDecibels(float percent)
    {
        return 80 * (percent - 1.0f);
    }
    private static float DecibelsToPercent(float db)
    {
        return Mathf.Pow(10, db / 40.0f);
    }
    */
    
    private static float PercentToDecibels(float percent)
    {
        return Mathf.Max(minimumDecibels, 40.0f * Mathf.Log10(percent));
    }
    private static float DecibelsToPercent(float db)
    {
        if (db == minimumDecibels)
        {
            return 0.0f;
        }
        else
        {
            return Mathf.Pow(10, db / 40.0f);
        }
    }

    private string GetFullParameterName()
    {
        return GetFullParameterName(mixer.name, mixerParameter);
    }

    private static string GetFullParameterName(string mixerName, string parameterName)
    {
        return mixerName + "." + parameterName;
    }

    private string GetFullParameterNameNotMuted()
    {
        return GetFullParameterName() + ".notMuted";
    }

    // Save the percent values of the slider.
    private void SaveValues()
    {
        PlayerPrefs.SetFloat(GetFullParameterName(), slider.value);
        PlayerPrefs.SetFloat(GetFullParameterNameNotMuted(), lastNotMutedValue);
    }

    private static void UpdateMixerValue(AudioMixer mixer, string parameterName, float percent)
    {
        float db = PercentToDecibels(percent);
        mixer.SetFloat(parameterName, db);
        //Debug.Log("AudioSlider.UpdateMixerValue: percent / db: " + percent + " / " + db);
    }
    
    // Set the mixer parameter corresponding to this slider.
    private void UpdateMixerValue()
    {
        UpdateMixerValue(mixer, mixerParameter, slider.value);
    }

    // Set the mixer parameter to the value that was saved.
    // If the value was not saved, don't change the mixer parameter.
    public static void LoadValueIntoMixer(AudioMixer mixer, string parameterName)
    {
        string key = GetFullParameterName(mixer.name, parameterName);
        if (PlayerPrefs.HasKey(key))
        {
            float value = PlayerPrefs.GetFloat(key);
            UpdateMixerValue(mixer, parameterName, value);
        }
    }

    // Get the value of the slider for muted volume.
    private float GetMuteVolume()
    {
        return slider.minValue;
    }

    // Method that the Slider component should call each time its value changes.
    public void SliderChangeValue()
    {
        UpdateMixerValue();
        OnMuteUpdated();
    }

    // Toggles mute status on the slider.
    public void ToggleMute()
    {
        float muteVolume = GetMuteVolume();
        if (slider.value == muteVolume)
        {
            slider.value = lastNotMutedValue;
        }
        else
        {
            slider.value = muteVolume;
        }
        OnValueSettled();
    }

    // Called when the player lets go of the slider handle.
    public void OnPointerUp(PointerEventData ped)
    {
        float muteVolume = GetMuteVolume();
        if (slider.value != muteVolume)
        {
            lastNotMutedValue = slider.value;
        }
        OnValueSettled();
    }

    private void OnMuteUpdated()
    {
        if (MuteUpdated != null)
        {
            MuteUpdated(slider.value == GetMuteVolume());
        }
    }

    // Called when the slider handle stops getting moved all over the place, settling on a value.
    // In other words...
    // Called when the user lets go of the slider or when the user calls ToggleMute.
    private void OnValueSettled()
    {
        if (saveValuesToDisk)
        {
            SaveValues();
        }
    }
}