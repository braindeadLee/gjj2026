using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using System;
using System.Collections.Generic;

[Serializable]
public struct AudioFile
{
    public AudioClip audio;
    public string name;
}

[Serializable]
public struct lineAndSub
{
    public string name;
    public string line;
}

public enum AudioCategory
{
    VoiceLines,
    StateSFX
}

public class AudioManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("----------Audio Source----------")]
    [SerializeField] public AudioSource musicSource;
    [SerializeField] public AudioSource SFXSource;

    [Header("----------SFX----------")]
    public AudioClip gamePlay_Music;
    public AudioClip menu_Music;
    public AudioClip intro_Music;
    public AudioFile[] voice_Lines;
    public AudioFile[] state_SFX; 
    
    public lineAndSub[] voiceAndSubs;/*Game State like Game Over, Victory, and Cue*/

    private bool inMenu = false;

    private Coroutine fadeCoroutine;

    public static AudioManager Instance {get; private set;}

    private void Awake()
    {
        if (Instance != null && Instance != this) 
            Destroy(this); 
        else 
            Instance = this; 
    }

    public void Start()
    {
        musicSource.clip = menu_Music;
        musicSource.volume = 0f;
        musicSource.loop = true;
        musicSource.Play();
    }

    private void Update()
    {
        var keyboard = Keyboard.current;

        if (keyboard.escapeKey.wasPressedThisFrame && inMenu == false)
        {
            inMenu = true;
            pause_SFX_MS();
        }else if(keyboard.escapeKey.wasPressedThisFrame && inMenu == true)
        {
            inMenu = false;
            unpause_SFX_MS();
        }

    }
    public void play_SFX(AudioClip clip, float volume = 1.0f)
    {
        SFXSource.PlayOneShot(clip, volume);
    }

    public void play_SFX(string clipName, AudioCategory category, float volume = 1.0f)
    {
        // Select the correct array based on the enum
        AudioFile[] targetArray = category == AudioCategory.VoiceLines ? voice_Lines : state_SFX;

        AudioFile file = targetArray.FirstOrDefault(f => f.name == clipName);

        if (file.audio != null)
        {
            SFXSource.PlayOneShot(file.audio, volume);
        }
        else
        {
            Debug.LogWarning($"Audio file '{clipName}' not found in {category}!");
        }
    }

    public void pause_SFX_MS()
    {
        musicSource.Pause();
        SFXSource.Pause();
    }

    public void unpause_SFX_MS()
    {
        musicSource.UnPause();
        SFXSource.UnPause();
    }

    public void SetMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void FadeMusic(float targetVolume, float duration)
    {
        // Stop any active fade to prevent conflicts
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        
        fadeCoroutine = StartCoroutine(FadeRoutine(targetVolume, duration));
    }

    private System.Collections.IEnumerator FadeRoutine(float targetVolume, float duration)
    {
        float startVolume = musicSource.volume;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            // Smoothly interpolate the volume
            musicSource.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            yield return null; // Wait for the next frame
        }

        musicSource.volume = targetVolume;
    }

    // Example: Update your pause logic to use fades
    public void SmoothPause(float duration)
    {
        StartCoroutine(PauseAfterFade(duration));
    }

    private System.Collections.IEnumerator PauseAfterFade(float duration)
    {
        yield return FadeRoutine(0, duration);
        musicSource.Pause();
    }
}
