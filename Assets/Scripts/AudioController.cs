using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioController : MonoBehaviour {
    public List<AudioClip> musicList;
    public List<AudioClip> sfxList;

    public List<AudioSource> Channels = new List<AudioSource>();

    public string[] PlaceTileSFX;
    public string[] RotateTileSFX;
    public string[] FlipBtnSFX;
    public string[] MenuClickSFX;
    public string[] TileSnapSFX;

    public string[] GeneralSFX;

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
        Scene currentScene = SceneManager.GetActiveScene();

        if (currentScene.name != "MainScene")
        {
            PlayMusic("Main_Menu_Music_1");
        }
        else
        {
            PlayMusic("Gameplay_Music_1");
        }
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

    public void PlayLoop(string sfxName)
    {
        foreach (AudioSource source in Channels)
        {
            if (source.clip.name == sfxName)
            {
                return;
            }
        }

        AudioClip clip = GetSFX(sfxName);

        if (clip != null)
        {
            StartCoroutine(PlayLoopChannel(clip));
        }
    }

    public void StopSFX(string sfxName)
    {
        foreach(AudioSource source in Channels)
        {
            if (source.clip.name == sfxName)
            {
                source.Stop();
                break;
            }
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

    public void FlipTile()
    {
        int index = UnityEngine.Random.Range(0, FlipBtnSFX.Length);

        PlaySFX(FlipBtnSFX[index]);
    }

    public void MenuClick()
    {
        int index = UnityEngine.Random.Range(0, MenuClickSFX.Length);

        PlaySFX(MenuClickSFX[index]);
    }

    public void SnapTile()
    {
        int index = UnityEngine.Random.Range(0, TileSnapSFX.Length);

        PlaySFX(TileSnapSFX[index]);
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

    IEnumerator PlayLoopChannel(AudioClip clip)
    {
        AudioSource tempChannel = null;
        tempChannel = gameObject.AddComponent<AudioSource>();
        Channels.Add(tempChannel);
        tempChannel.clip = clip;
        tempChannel.loop = true;
        tempChannel.Play();

        while (tempChannel.isPlaying)
        {
            yield return null;
        }
        
        if (tempChannel != null)
        {
            Channels.Remove(tempChannel);
            Destroy(tempChannel);
        }
    }

}
