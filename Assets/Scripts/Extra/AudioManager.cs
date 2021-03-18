using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;
using UnityEngine.UIElements;

public class AudioManager : MonoBehaviour
{
    public AudioClips[] aud;
    public AudioSource audioSource;

    public Slider musicVolume;
    public Slider soundVolume;

    public float mVol = 1;
    public float sVol = 1;
    public float masVol = 1;

    [System.Serializable]
    public class AudioClips
    {
        public string clipName;
        public AudioClip audioClip;
        public float clipVolume = 1;
        public bool loop;
        public bool music;

        public AudioSource audioSource;
    };

    public static AudioManager existant;

    private void Awake()
    {
        //Creates a new audiomanager if there isn't one already
        if (existant == null)
        {
            existant = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(this.gameObject);

        //adds all audio clips
        foreach (AudioClips audio in aud)
        {
            audio.audioSource = gameObject.AddComponent<AudioSource>();
            audio.audioSource.clip = audio.audioClip;

            audio.audioSource.volume = audio.clipVolume;
            audio.audioSource.loop = audio.loop;
        }
    }

    private void Start()
    {
        mVol = 1;
        sVol = 1;
        masVol = 1;
        PlaySound("MusicMenu");

    }

    /// <summary>
    /// Plays sound "audioName"
    /// </summary>
    /// <param name="audioName"></param>
    public void PlaySound(string audioName)
    {
        AudioClips audioFound = Array.Find(aud, AudioClips => AudioClips.clipName == audioName);
        if (audioFound != null)
        {
            audioFound.audioSource.Play();
        }
    }

    public void PlaySoundOnly(string audioName)
    {
        AudioClips audioFound = Array.Find(aud, AudioClips => AudioClips.clipName == audioName);
        if (audioFound != null && !audioFound.audioSource.isPlaying)
        {
            audioFound.audioSource.Play();
        }
    }

    /// <summary>
    /// Sets music volume, best to use slider ui to set
    /// </summary>
    /// <param name="vol"></param>
    public void MusicVolume(float vol)
    {
        mVol = vol;
        ChangeVol();
    }
    /// <summary>
    /// Sets sound effects volume, best to use slider ui to set
    /// </summary>
    /// <param name="vol"></param>
    public void SoundVolume(float vol)
    {
        sVol = vol;
        ChangeVol();
    }
    /// <summary>
    /// Sets master volume, best to use slider ui to set
    /// </summary>
    /// <param name="vol"></param>
    public void MasterVolume(float vol)
    {
        masVol = vol;
        foreach (AudioClips audio in aud)
        {
            audio.audioSource.volume = mVol * masVol;
            audio.audioSource.volume = sVol * masVol;
        }
    }

    /// <summary>
    /// Changes sound and music volume. Must find a way to show something is a music track rather than using audio.loop
    /// </summary>
    public void ChangeVol()
    {
        foreach (AudioClips audio in aud)
        {
            if (audio.music == true)
            {
                audio.audioSource.volume = mVol * masVol;
            }
            else
            {
                audio.audioSource.volume = sVol * masVol;
            }
        }
    }
    ///<summary>
    ///Starts music for the level if track name is "MusicGame" Stops any "MusicMenu"
    ///</summary>

    public void ActivateGameMusic()
    {
        AudioClips audioFound = Array.Find(aud, AudioClips => AudioClips.clipName == "MusicMenu");
        if (audioFound != null)
        {
            audioFound.audioSource.Stop();
        }
        audioFound = Array.Find(aud, AudioClips => AudioClips.clipName == "MusicGame");
        if (audioFound != null)
        {
            audioFound.audioSource.Play();
        }
    }

    /// <summary>
    /// Starts music for the menu if there is a track name is "MusicMenu" Stops any "MusicGame"
    /// </summary>
    public void ActivateMenuMusic()
    {
        AudioClips audioFound = Array.Find(aud, AudioClips => AudioClips.clipName == "MusicGame");
        if (audioFound != null)
        {
            audioFound.audioSource.Stop();
        }
        audioFound = Array.Find(aud, AudioClips => AudioClips.clipName == "MusicMenu");
        if (audioFound != null)
        {
            audioFound.audioSource.Play();
        }
    }

    /// <summary>
    /// Stops all audio from playing
    /// </summary>
    public void StopAllSound()
    {
        foreach (AudioClips audio in aud)
        {
            audio.audioSource.Stop();
        }
    }

    public void StopSound(string audioName)
    {
        AudioClips audioFound = Array.Find(aud, AudioClips => AudioClips.clipName == audioName);
        if (audioFound != null && audioFound.audioSource.isPlaying)
        {
            audioFound.audioSource.Stop();
        }
    }
}
