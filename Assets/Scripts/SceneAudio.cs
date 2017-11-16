// Author(s): Paul Calande
// Starts playing audio when the scene begins.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneAudio : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The music that plays for this scene.")]
    AudioClip sceneMusic;

    void Start ()
    {
        AudioController ac = AudioController.Instance;
        ac.PlayMusic(sceneMusic);
	}
}