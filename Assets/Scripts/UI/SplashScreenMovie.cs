// Author(s): Paul Calande
// Script for the splash screen and movie.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashScreenMovie : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Reference to the video player.")]
    VideoPlayer videoPlayer;
    [SerializeField]
    [Tooltip("Reference to the AudioSource.")]
    AudioSource audioSource;
    [SerializeField]
    [Tooltip("Reference to the splash screen image.")]
    Image splashScreen;
    [SerializeField]
    [Tooltip("The minimum amount of time to wait before playing the splash screen.")]
    float secondsToWait = 2.0f;

    // Whether or not the video is prepared yet.
    bool videoPrepared = false;
    // Whether or not the video is playing.
    bool videoIsPlaying = false;
    // The number of seconds waited since the video has started preparing.
    float secondsWaited = 0.0f;

    private void Start()
    {
        videoPlayer.loopPointReached += VideoPlayer_LoopPointReached;
        videoPlayer.prepareCompleted += VideoPlayer_PrepareCompleted;
        videoPlayer.started += VideoPlayer_Started;
        videoPlayer.Prepare();
    }

    private void Update()
    {
        secondsWaited += Time.deltaTime;
        if (!videoIsPlaying && videoPrepared && secondsWaited > secondsToWait)
        {
            StartVideo();
            videoIsPlaying = true;
        }
    }

    private void StartVideo()
    {
        videoPlayer.Play();
        audioSource.Play();
    }

    private void VideoPlayer_Started(VideoPlayer vp)
    {
        splashScreen.enabled = false;
    }

    private void VideoPlayer_PrepareCompleted(VideoPlayer vp)
    {
        videoPrepared = true;
    }

    private void VideoPlayer_LoopPointReached(VideoPlayer vp)
    {
        SceneManager.LoadScene("Title");
    }
}