﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

/// <summary>
/// The AudioManager will store all audio inside AudioClips. You can then call the public functions and play sound or music by the name.
/// <para>There can only be one AudioManager at a time</para>
/// </summary>
public class AudioManager : MonoBehaviour
{
    [Header("Audio floats")]
    //Dynamic Floats
    [SerializeField] [Tooltip("Music Volume, dynamically used by sliders")] [Range(0,1)] public float mVol;
    [SerializeField] [Tooltip("Sound Volume,dynamically used by sliders")] [Range(0, 1)] public float sVol;
    [SerializeField] [Tooltip("Master Volume, dynamically used by sliders")] [Range(0, 1)] public float masVol;
    [Space]

    //Audio management
    [SerializeField] [Tooltip("All sound is stored inside this")] public AudioClips[] aud;

    /// <summary>
    /// AudioClips is used to store details about a track. It has:
    /// <list type="bullet">
    ///<listheader>
    ///    <item>Clip name</item>
    ///    <item>Audio clip</item><description>- The actual sound file</description>
    ///    <item>Clip volume</item>
    ///    <item>Loop?</item>
    ///    <item>Music?</item>
    ///</listheader>
    ///</list>
    ///<para>It's unity readable form is stored in AudioSource</para>
    ///<para>This should then be stored in an array and can be found and
    ///played from there thanks to these details</para>
    /// </summary>
    [System.Serializable]
    public class AudioClips
    {
        //Settings of the AudioClip
        [SerializeField] [Tooltip("Name to call Play")] public string clipName;
        [SerializeField] [Tooltip("The audio file is stored here")] public AudioClip audioClip;
        [SerializeField] [Tooltip("Adjust the volume of this sound")] public float clipVolume = 1;
        [SerializeField] [Tooltip("Does this clip be looping?")] public bool loop;
        [SerializeField] [Tooltip("Is this audio a music piece?")] public bool music;

        public AudioSource audioSource;
    };


    public static AudioManager existant;

    /// <summary>
    /// Destroys self if duplicate. //Loads all audio files in automatically. 
    /// </summary>
    private void Awake()
    {
        //TODO: Load all sfx by file automatically

        //Creates a new audiomanager if there isn't one already
        if (existant == null){existant = this;}
        else{Destroy(this.gameObject); return; }

        DontDestroyOnLoad(this.gameObject);

        //Has to set the settings of AudioClips to the AudioSource in AudioClips
        foreach (AudioClips audio in aud)
        {
            audio.audioSource = gameObject.AddComponent<AudioSource>();
            audio.audioSource.clip = audio.audioClip;

            audio.audioSource.volume = audio.clipVolume;
            audio.audioSource.loop = audio.loop;
        }
    }


    /// <summary>
    /// Starts menumusic and sets the initial sound volumes.
    /// </summary>
    private void Start()
    {
        ActivateMenuMusic();
        MusicVolume(0.5f);
        SoundVolume(0.5f);
        MasterVolume(0.1f);
    }

    #region VOLUME_CONTROL
    /// <summary>
    /// Manually sets music volume
    /// </summary>
    /// <param name="vol"></param>
    public void MusicVolume(float vol)
    {
        mVol = vol;
        ChangeVol();
    }
    /// <summary>
    /// Manually sets sound effects volume
    /// </summary>
    /// <param name="vol"></param>
    public void SoundVolume(float vol)
    {
        sVol = vol;
        ChangeVol();
    }
    /// <summary>
    /// Manually sets master volume
    /// </summary>
    /// <param name="vol"></param>
    public void MasterVolume(float vol)
    {
        masVol = vol;
        ChangeVol();
    }

    /// <summary>
    /// Updates the audioSources volumes.
    /// </summary>
    private void ChangeVol()
    {
        //Changes all the volumes of every audioclip in AudioClips, keeping in respect to the base value
        foreach (AudioClips audio in aud)
        {
            if (audio.music == true)
            {
                audio.audioSource.volume = mVol * masVol * audio.clipVolume;
            }
            else
            {
                audio.audioSource.volume = sVol * masVol * audio.clipVolume;
            }
        }
    }
    #endregion

    #region PlaySFX
    /// <summary>
    /// Plays a sound effect "audioName"
    /// <para>Can be interupted by self</para>
    /// </summary>
    public void PlaySound(string audioName)
    {
        AudioClips audioFound = Array.Find(aud, AudioClips => AudioClips.clipName == audioName);
        if (audioFound != null && !audioFound.music)
        {
            audioFound.audioSource.Play();
        }
    }
    ///<summary>
    /// Plays a sound effect "audioName"
    /// <para>Cannot be interupted by self</para>
    /// </summary>
    public void PlaySoundOnly(string audioName)
    {
        AudioClips audioFound = Array.Find(aud, AudioClips => AudioClips.clipName == audioName);
        if (audioFound != null && !audioFound.audioSource.isPlaying && !audioFound.music)
        {
            audioFound.audioSource.Play();
        }
    }

    //Todo: Play music by name instead

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
        Debug.Log("Activating Menu Music");
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

    #endregion

    //Todo: Rename StopAllSound to StopAllAudio. Keep it consistant, sound - music - audio

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
    /// <summary>
    /// Stops any audio that is playing and has the parameter clipName
    /// </summary>
    /// <param name="audioName"></param>
    public void StopSound(string audioName)
    {
        AudioClips audioFound = Array.Find(aud, AudioClips => AudioClips.clipName == audioName);
        if (audioFound != null && audioFound.audioSource.isPlaying)
        {
            audioFound.audioSource.Stop();
        }
    }






    //Todo: Rename Panel sections, and maybe re-do functions to think of including another settings panel e.g. the pause menu


    [SerializeField] [Tooltip("New canvas")] private GameObject canvas;
    private GameObject returningCanvas;
    [SerializeField] [Tooltip("Settings panel")] private GameObject settingsPanel;

    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private GameObject visualsPanel;
    [SerializeField] private GameObject audioPanel;



    public void PanelSettings(bool on)
    {
        //GetComponent<VisualSettings>().UpdateVisualSettings();
        //GetComponent<ControlsSettings>().UpdateControls();

        canvas.SetActive(on);
        settingsPanel.SetActive(on);

        Canvas[] g = GameObject.FindObjectsOfType<Canvas>();
        foreach (var item in g)
        {
            if (item == canvas.GetComponent<Canvas>())
            {
                item.gameObject.SetActive(on);
            }
            else
            {
                returningCanvas = item.gameObject;
                item.gameObject.SetActive(!on);

            }
        }
        if (!on)
        {
            returningCanvas.SetActive(!on);
        }
    }

    public void PanelControls(bool on)
    {
        controlsPanel.SetActive(on);
        visualsPanel.SetActive(!on);
        audioPanel.SetActive(!on);
    }
    public void PanelVisual(bool on)
    {
        controlsPanel.SetActive(!on);
        visualsPanel.SetActive(on);
        audioPanel.SetActive(!on);
    }
    public void PanelAudio(bool on)
    {
        controlsPanel.SetActive(!on);
        visualsPanel.SetActive(!on);
        audioPanel.SetActive(on);
    }
}