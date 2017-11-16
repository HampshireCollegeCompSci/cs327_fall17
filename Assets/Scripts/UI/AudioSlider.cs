using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AudioSlider : MonoBehaviour  {

    public Slider thisSlider;

    public Image btn;

    public bool music;
    public bool sfx;

    private void Start()
    {
        float value;

        if (music)
        {
            AudioController.Instance.sfxGroup.audioMixer.GetFloat("musicVol", out value);
            if (value == -80f)
            {
                btn.color = Color.red;
            }
            else
            {
                btn.color = Color.white;
            }
        }

        if (sfx)
        {
            AudioController.Instance.sfxGroup.audioMixer.GetFloat("sfxVol", out value);
            if (value == -80f)
            {
                btn.color = Color.red;
            }
            else
            {
                btn.color = Color.white;
            }
        }
    }

    public void ChangeMusicVol()
    {
        AudioController.Instance.sfxGroup.audioMixer.SetFloat("musicVol", thisSlider.value);

        float value;
        AudioController.Instance.sfxGroup.audioMixer.GetFloat("musicVol", out value);
        if (value == -80f)
        { 
            btn.color = Color.red;
        }
        else
        {
            btn.color = Color.white;
        }

    }

    public void ChangeSFXVol()
    {
        AudioController.Instance.sfxGroup.audioMixer.SetFloat("sfxVol", thisSlider.value);

        float value;
        AudioController.Instance.sfxGroup.audioMixer.GetFloat("sfxVol", out value);
        if (value == -80f)
        {
            btn.color = Color.red;
        }
        else
        {
            btn.color = Color.white;
        }
    }

    public void MuteMusic()
    {
        float value;
        AudioController.Instance.sfxGroup.audioMixer.GetFloat("musicVol", out value);
        if (value == -80f)
        {
            AudioController.Instance.sfxGroup.audioMixer.SetFloat("musicVol", 0);
            thisSlider.value = 0;
        }
        else
        {
            AudioController.Instance.sfxGroup.audioMixer.SetFloat("musicVol", -80f);
        }
        
        AudioController.Instance.sfxGroup.audioMixer.GetFloat("musicVol", out value);
        if (value == -80f)
        {
            btn.color = Color.red;
        }
        else
        {
            btn.color = Color.white;
        }
    }

    public void MuteSFX()
    {
        float value;
        AudioController.Instance.sfxGroup.audioMixer.GetFloat("sfxVol", out value);
        if (value == -80f)
        {
            AudioController.Instance.sfxGroup.audioMixer.SetFloat("sfxVol", 0);
            thisSlider.value = 0;
        }
        else
        {
            AudioController.Instance.sfxGroup.audioMixer.SetFloat("sfxVol", -80f);
        }

        AudioController.Instance.sfxGroup.audioMixer.GetFloat("sfxVol", out value);
        if (value == -80f)
        {
            btn.color = Color.red;
        }
        else
        {
            btn.color = Color.white;
        }
    }
}
