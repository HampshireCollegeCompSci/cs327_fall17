// Author(s): Paul Calande
// A mute button class that subscribes to an AudioSlider.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuteButton : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The audio slider to mute and unmute.")]
    AudioSlider audioSlider;
    [SerializeField]
    [Tooltip("The crossed-out image over the mute button to enable and disable.")]
    GameObject muteButtonCrossout;

    // Use this for initialization
    void Awake()
    {
        audioSlider.MuteUpdated += AudioSlider_MuteUpdated;
    }

    void AudioSlider_MuteUpdated(bool isMuted)
    {
        muteButtonCrossout.SetActive(isMuted);
    }
}