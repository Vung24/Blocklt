using UnityEngine;
using System;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    private AudioSource bgmSource;
    private AudioSource sfxSource;

    private Dictionary<string, Sound> soundDict;

    private bool bgmEnabled;
    private bool sfxEnabled;

    public static AudioManager instance;
    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        InitPlayerPref();
    }
    void InitPlayerPref()
    {
        PlayerPrefs.SetInt("countP1", 0);
        PlayerPrefs.SetInt("countP2", 0);
        bgmEnabled = PlayerPrefs.GetInt("BGM_ENABLED", 1) == 1;
        sfxEnabled = PlayerPrefs.GetInt("SFX_ENABLED", 1) == 1;
    }
    void Start()
    {
        bgmSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();

        soundDict = new Dictionary<string, Sound>();
        foreach (Sound s in sounds)
        {
            soundDict[s.name] = s;
        }

        UpdateVolume();
    }
    private void UpdateVolume()
    {
        bgmSource.volume = bgmEnabled ? 1f : 0f;
        sfxSource.volume = sfxEnabled ? 1f : 0f;
    }

    public void PlaySFX(string soundName)
    {
        if (!sfxEnabled) return;

        if (!soundDict.TryGetValue(soundName, out Sound s))
        {
            Debug.LogWarning($"SFX '{soundName}' not found!");
            return;
        }

        sfxSource.pitch = s.pitch;
        sfxSource.PlayOneShot(s.clip, s.volume);
    }
    public void PlayBGM(string soundName)
    {
        if (!soundDict.TryGetValue(soundName, out Sound s))
        {
            Debug.LogWarning($"BGM '{soundName}' not found!");
            return;
        }

        bgmSource.clip = s.clip;
        bgmSource.volume = s.volume;
        bgmSource.pitch = s.pitch;
        bgmSource.loop = true;

        if (bgmEnabled)
        {
            bgmSource.Play();
        }
    }
    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void ToggleBGM()
    {
        bgmEnabled = !bgmEnabled;
        PlayerPrefs.SetInt("BGM_ENABLED", bgmEnabled ? 1 : 0);
        PlayerPrefs.Save();

        if (bgmEnabled)
        {
            bgmSource.Play();
        }
        else
        {
            bgmSource.Stop();
        }
    }

    public void ToggleSFX()
    {
        sfxEnabled = !sfxEnabled;
        PlayerPrefs.SetInt("SFX_ENABLED", sfxEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public bool IsBGMOn() => bgmEnabled;
    public bool IsSFXOn() => sfxEnabled;
}
