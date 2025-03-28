using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using WaterSort;

public class SoundController : Singleton<SoundController>
{
    private Dictionary<AUDIO_KEY, AudioClip> dictSounds;
    public SoundConfig soundConfig;

    private SoundScript objMusic;
    private List<SoundScript> objSounds;
    private SoundScript tempSound;

    private bool isSound = true;
    private bool isMusic = true;

    public void Start()
    {
        SetMusic(AudioManager.IsMusicEnable);
        SetSound(AudioManager.IsSoundEnable);

        dictSounds = new Dictionary<AUDIO_KEY, AudioClip>();
        for (int i = 0; i < soundConfig.sounds.Length; i++)
        {
            dictSounds.Add(soundConfig.sounds[i].key, soundConfig.sounds[i].clip);
        }

        objSounds = new List<SoundScript>();
        AudioManager.SoundStateChanged += AudioManager_SoundStateChanged;
        AudioManager.MusicStateChanged += AudioManager_MusicStateChanged;
        PlayMusic(AUDIO_KEY.MUSIC_BACKGROUND);
    }
    private void OnDestroy()
    {
        AudioManager.SoundStateChanged -= AudioManager_SoundStateChanged;
        AudioManager.MusicStateChanged -= AudioManager_MusicStateChanged;
    }

    private void AudioManager_MusicStateChanged(bool obj)
    {
        SetMusic(obj);
    }

    private void AudioManager_SoundStateChanged(bool obj)
    {
        SetSound(obj);
    }

    private AudioClip GetAudioClip(AUDIO_KEY nameFile)
    {
        if (dictSounds.ContainsKey(nameFile))
        {
            return dictSounds[nameFile];
        }
        Debug.LogError("WHY---!!!");
        return null;
    }

    public void SetMusic(bool isOn)
    {
        this.isMusic = isOn;
        if (isOn)
            ResumeMusic();
        else StopMusic();
    }

    public void PlayMusic(AUDIO_KEY nameClip)
    {
        if (objMusic == null)
        {
            GameObject temp = new GameObject("MUSIC", typeof(SoundScript));
            objMusic = temp.GetComponent<SoundScript>();
            objMusic.AddAudioSource(true);
        }
        objMusic.PlayMusic(nameClip, GetAudioClip(nameClip));
        objMusic.SetMute(!isMusic);
    }
    public void MuteMusic(bool isMute)
    {
        if (isMusic)
        {
            if (objMusic != null) objMusic.SetMute(isMute);
        }
    }
    public void ResumeMusic()
    {
        if (objMusic != null)
        {
            objMusic.Resume(!isMusic);
        }
    }
    public void StopMusic()
    {
        if (objMusic != null)
        {
            objMusic.Stop(!isMusic);
        }
    }
    public void DestroyMusic()
    {
        if (objMusic != null) Destroy(objMusic.gameObject);
    }

    public void SetSound(bool isOn)
    {
        this.isSound = isOn;
    }
    public void PlaySound(AUDIO_KEY nameClip, float volume = 1f)
    {
        if (!isSound) return;
        tempSound = null;
        for (int i = 0; i < objSounds.Count; i++)
        {
            if (!objSounds[i].gameObject.activeSelf)
            {
                tempSound = objSounds[i];
            }
        }
        if (tempSound == null)
        {
            var temp = new GameObject("SOUND", typeof(SoundScript));
            tempSound = temp.GetComponent<SoundScript>();
            tempSound.AddAudioSource(false);
            objSounds.Add(tempSound);
        }

        tempSound.SetVolume(volume);
        tempSound.PlaySound(nameClip, GetAudioClip(nameClip));
    }

    public void StopSound(AUDIO_KEY nameClip)
    {
        for (int i = 0; i < objSounds.Count; i++)
        {
            if (objSounds[i].gameObject.activeSelf)
            {
                if (objSounds[i].nameClip == nameClip)
                {
                    objSounds[i].Stop();
                    return;
                }
            }
        }
    }
}
