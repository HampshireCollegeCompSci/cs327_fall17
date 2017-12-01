// Author(s): Joel Esquilin, Paul Calande
// An audio controller class with a few too many responsibilities.
// Ideally, an audio controller class should only perform channel management.
// The AudioClips should be kept elsewhere, namely on the objects that should play them.
// The AudioClips should be passed to this AudioController for channel creation and playback.
// Alas, it's likely too late in the development cycle to change that.

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Audio;
//using UnityEngine.SceneManagement;

public class AudioController : MonoBehaviour
{
    //public List<AudioClip> musicList;
    public List<AudioClip> sfxList;

    public List<AudioSource> Channels = new List<AudioSource>();

    public string[] PlaceTileSFX;
    public string[] RotateTileSFX;
    public string[] FlipBtnSFX;
    public string[] MenuClickSFX;
    public string[] TileSnapSFX;
    public string[] OutlineSFX;

    public string[] GeneralSFX;

    public AudioMixerGroup musicGroup;
    public AudioMixerGroup sfxGroup;

    //public AudioSource currentlyPlaying;

    AudioMixer thisMixer;

    [SerializeField]
    [Tooltip("Reference to the music audio source.")]
    AudioSource musicSource;

    [SerializeField]
    GameObject go;

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
            //instance.PlayMusic(sceneMusic);

            DestroyImmediate(gameObject);
            return;
        }
        instance = this;

        //PlayMusic(sceneMusic);

        thisMixer = musicGroup.audioMixer;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Unfortunately, mixer values cannot be set within Awake.
        AudioSlider.LoadValueIntoMixer(thisMixer, "musicVol");
        AudioSlider.LoadValueIntoMixer(thisMixer, "sfxVol");
    }

    public void PlayMusic(AudioClip clip)
    {
        if (musicSource.clip != clip)
        {
            musicSource.Stop();
            musicSource.clip = clip;
            musicSource.Play();
        }
    }

    public void StopMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
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

    /*
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
    */

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

    public void GameOver()
    {
        PlaySFX("Game_Over_2");
    }

    public void PressPlay()
    {
        PlaySFX("Press_Play_1");
    }

    public void PickupTile()
    {
        PlaySFX("Tiles_Pickup_1");
    }

    public void Lightning()
    {
        PlaySFX("Clear_Square_Lightning");
    }

    public void Outline()
    {
        //PlaySFX("Clear_Square_Outline");
        int index = UnityEngine.Random.Range(0, OutlineSFX.Length);

        PlaySFX(OutlineSFX[index]);
    }

    public void WinGame()
    {
        PlaySFX("Game_Win");
    }

    public void StartEventGroup(VoidEventGroup.EventGroupType eventType)
    {
        switch (eventType)
        {
            case VoidEventGroup.EventGroupType.Junkyard:
                PlaySFX("Unrefined_Uranium_1");
                break;

            case VoidEventGroup.EventGroupType.Radiation:
                PlaySFX("Waste_Contamination_1");
                break;

            case VoidEventGroup.EventGroupType.Asteroids:
                PlaySFX("Reactor_Breach_1");
                break;
        }
    }

    public void StopAllSFX()
    {
        foreach(AudioSource channel in Channels)
        {
            if (sfxList.Contains(channel.clip))
            {
                channel.Stop();
            }
        }
    }

    IEnumerator PlayTempChannel(AudioClip clip)
    {
        AudioSource tempChannel = null;
        float remainingTime = clip.length;
        tempChannel = gameObject.AddComponent<AudioSource>();
        Channels.Add(tempChannel);
        tempChannel.clip = clip;
        tempChannel.outputAudioMixerGroup = sfxGroup;
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
        tempChannel.outputAudioMixerGroup = sfxGroup;
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

    /*
    private void SceneManager_SceneChanged(Scene from, Scene to)
    {

    }
    */
}