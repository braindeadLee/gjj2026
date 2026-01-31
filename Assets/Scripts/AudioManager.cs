using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class AudioManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("----------Audio Source----------")]
    [SerializeField] public AudioSource musicSource;
    [SerializeField] public AudioSource SFXSource;

    [Header("----------SFX----------")]
    public AudioClip gamePlay_Music;
    public AudioClip menu_Music;
    public AudioClip[] voice_Lines;
    public AudioClip[] generic_Comments;
    public AudioClip[] special_Comments;
    public AudioClip[] state_SFX; /*Game State like Game Over, Victory, and Cue*/
    public AudioClip[] npc_Movement;
    public AudioClip[] button_SFX;

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

    public void play_SFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
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
