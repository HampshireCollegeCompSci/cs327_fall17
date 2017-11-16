// Author(s): Paul Calande
// Script for the splash screen movie.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class SplashScreenMovie : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Reference to the video player.")]
    VideoPlayer videoPlayer;

    private void Start()
    {
        videoPlayer.loopPointReached += VideoFinished;
        videoPlayer.Play();
    }

    private void VideoFinished(VideoPlayer vp)
    {
        SceneManager.LoadScene("Title");
    }
}