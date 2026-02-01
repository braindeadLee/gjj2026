using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using System;

[Serializable]
public struct AudioFile
{
    public AudioClip audio;
    public string name;
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
    public AudioFile[] voice_Lines;
    public AudioFile[] state_SFX; /*Game State like Game Over, Victory, and Cue*/

    private bool inMenu = false;

    private void Start()
    {
        musicSource.clip = gamePlay_Music;
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

    public void play_SFX(string clipName)
{
    // Find the first struct where the name matches
    AudioFile file = voice_Lines.FirstOrDefault(f => f.name == clipName);

    if (file.audio != null)
    {
        SFXSource.PlayOneShot(file.audio);
    }
    else
    {
        Debug.LogWarning($"Audio file '{clipName}' not found!");
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



}
