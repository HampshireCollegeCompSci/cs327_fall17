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
    [SerializeField]
    [Tooltip("Reference to the AudioSource.")]
    AudioSource audioSource;

    private void Start()
    {
        videoPlayer.loopPointReached += VideoFinished;
        videoPlayer.prepareCompleted += VideoPrepared;
        videoPlayer.Prepare();
    }

    private void VideoPrepared(VideoPlayer vp)
    {
        videoPlayer.Play();
        audioSource.Play();
    }

    private void VideoFinished(VideoPlayer vp)
    {
        SceneManager.LoadScene("Title");
    }
}