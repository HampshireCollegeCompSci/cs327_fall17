using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour {
    public List<AudioClip> musicList;
    public List<AudioClip> sfxList;

    public List<AudioSource> Channels = new List<AudioSource>();

    public string[] PlaceTileSFX = new string[5];
    public string[] RotateTileSFX = new string[2];

    private AudioSource currentlyPlaying;

    private static AudioController instance = null;
    public static AudioController Instance
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

    private void Start()
    {
        PlayMusic("Main_Menu_Music_1");
    }

    public void PlayMusic(string musicName)
    {
        AudioClip clip = GetMusic(musicName);

        if (clip != null)
        {
            AudioSource newChannel = gameObject.AddComponent<AudioSource>();
            Channels.Add(newChannel);
            newChannel.clip = clip;
            newChannel.loop = true;
            newChannel.Play();
            currentlyPlaying = newChannel;
        }
    }

    public void StopMusic()
    {
        if (currentlyPlaying != null)
        {
            currentlyPlaying.Stop();
            Channels.Remove(currentlyPlaying);
        }
    }

    public void PlaySFX(string sfxName)
    {
        AudioClip clip = GetSFX(sfxName);

        if (clip != null)
        {
            StartCoroutine(PlayTempChannel(clip));
        }
    }

    AudioClip GetMusic(string musicName)
    {
        foreach (AudioClip clip in musicList)
        {
            if (clip.name == musicName)
            {
                return clip;
            }
        }

        return null;
    }

    AudioClip GetSFX(string sfxName)
    {
        foreach(AudioClip clip in sfxList)
        {
            if (clip.name == sfxName)
            {
                return clip;
            }
        }

        return null;
    }

    public void PlaceTile()
    {
        int index = UnityEngine.Random.Range(0, PlaceTileSFX.Length);

        PlaySFX(PlaceTileSFX[index]);
    }

    public void RotateTile()
    {
        int index = UnityEngine.Random.Range(0, RotateTileSFX.Length);

        PlaySFX(RotateTileSFX[index]);
    }

    IEnumerator PlayTempChannel(AudioClip clip)
    {
        AudioSource tempChannel = null;
        float remainingTime = clip.length;
        tempChannel = gameObject.AddComponent<AudioSource>();
        Channels.Add(tempChannel);
        tempChannel.clip = clip;
        tempChannel.Play();

        yield return new WaitForSeconds(remainingTime);

        if (tempChannel != null)
        {
            Channels.Remove(tempChannel);
            Destroy(tempChannel);
        }
    }
    
}
