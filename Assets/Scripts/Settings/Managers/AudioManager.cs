using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : BaseSettingsManager<AudioManager>
{
    public event Action OnAudioSettingsChanged;

    private const string SECTION = "Audio";

    private const string MASTER = "Master";
    private const string MUSIC = "Music";
    private const string SFX = "SFX";
    private const string VOICE = "Voice";

    [Header("Mixer")]
    [SerializeField] private AudioMixer mixer;
        
    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource voiceSource;
        
    [Header("Test Audio Clips")]
    [SerializeField] private AudioClip testMusicClip;
    [SerializeField] private AudioClip testSFXClip;
    [SerializeField] private AudioClip testVoiceClip;

    // Exposed parameter names in Unity AudioMixer
    private const string MASTER_VOL = "MasterVolume";
    private const string MUSIC_VOL = "MusicVolume";
    private const string SFX_VOL = "SFXVolume";
    private const string VOICE_VOL = "VoiceVolume";

    #region Initialization

    protected override void CreateDefaults()
    {
        SetVolume(MASTER, 1f);
        SetVolume(MUSIC, 1f);
        SetVolume(SFX, 1f);
        SetVolume(VOICE, 1f);
    }

    protected override void OnLoaded()
    {
        ApplyAllVolumes();
    }

    protected override void OnReverted()
    {
        ApplyAllVolumes();
    }

    #endregion

    #region Public API

    public void SetMasterVolume(float value) => SetAndApply(MASTER, MASTER_VOL, value);
    public void SetMusicVolume(float value) => SetAndApply(MUSIC, MUSIC_VOL, value);
    public void SetSFXVolume(float value) => SetAndApply(SFX, SFX_VOL, value);
    public void SetVoiceVolume(float value) => SetAndApply(VOICE, VOICE_VOL, value);

    public float GetMasterVolume() => GetVolume(MASTER, 1f);
    public float GetMusicVolume() => GetVolume(MUSIC, 1f);
    public float GetSFXVolume() => GetVolume(SFX, 1f);
    public float GetVoiceVolume() => GetVolume(VOICE, 1f);

    #endregion

    #region Core Logic

    private void SetAndApply(string key, string mixerParam, float value)
    {
        value = Mathf.Clamp01(value);

        SetVolume(key, value);
        ApplyVolume(mixerParam, value);

        OnAudioSettingsChanged?.Invoke();
    }

    private void ApplyAllVolumes()
    {
        ApplyVolume(MASTER_VOL, GetVolume(MASTER, 1f));
        ApplyVolume(MUSIC_VOL, GetVolume(MUSIC, 1f));
        ApplyVolume(SFX_VOL, GetVolume(SFX, 1f));
        ApplyVolume(VOICE_VOL, GetVolume(VOICE, 1f));
    }

    private void ApplyVolume(string exposedParam, float value)
    {
        if (mixer == null)
        {
            Debug.LogWarning("AudioMixer not assigned!");
            return;
        }

        float db = LinearToDecibel(value);
        mixer.SetFloat(exposedParam, db);
    }

    #endregion

    #region Storage Helpers

    private void SetVolume(string key, float value)
    {
        SetValue(SECTION, key, value.ToString());
    }

    private float GetVolume(string key, float defaultValue)
    {
        return GetFloat(SECTION, key, defaultValue);
    }

    #endregion

    #region Utilities

    // Converts 0–1 linear slider to decibel (-80dB to 0dB)
    private float LinearToDecibel(float value)
    {
        if (value <= 0.0001f)
            return -80f;

        return Mathf.Log10(value) * 20f;
    }

    #endregion
    
    #region Test Methods
    public void TestMusicClip()
    {
        if (testMusicClip != null && musicSource != null)
        {
            musicSource.clip = testMusicClip;
            musicSource.Play();
            Debug.Log("Playing test music clip");
        }
        else
        {
            Debug.LogWarning("Test music clip or music source not assigned");
        }
    }
        
    public void TestSFXClip()
    {
        if (testSFXClip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(testSFXClip);
            Debug.Log("Playing test SFX clip");
        }
        else
        {
            Debug.LogWarning("Test SFX clip or SFX source not assigned");
        }
    }
        
    public void TestVoiceClip()
    {
        if (testVoiceClip != null && voiceSource != null)
        {
            voiceSource.PlayOneShot(testVoiceClip);
            Debug.Log("Playing test voice clip");
        }
        else
        {
            Debug.LogWarning("Test voice clip or voice source not assigned");
        }
    }
    
    public void TestStopClips()
    {
        if (testMusicClip != null && musicSource != null)
        {
            musicSource.Stop();
        }
        if (testSFXClip != null && sfxSource != null)
        {
            sfxSource.Stop();
        }
        if (testVoiceClip != null && voiceSource != null)
        {
            voiceSource.Stop();
        }
    }
    
    #endregion
}