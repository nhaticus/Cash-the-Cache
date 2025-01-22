using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
HOW TO USE SFX:
upon collision/movement call: AudioManager.Instance.PlaySFX("name_of_clip");
HOW TO STOP IDLE MUSIC:
upon start game, etc. call: AudioManager.Instance.musicSOurce.Stop();
*/

public class AudioManager : MonoBehaviour
{

    public static AudioManager Instance;

    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start() {
        PlayMusic("idle_music");
        // make the music loop
        musicSource.loop = true;
    }

    public void PlayMusic(string name){
        Sound s = System.Array.Find(musicSounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("Sound: " + name + " not found!");
            return;
        }
        else{
            musicSource.clip = s.clip;
            musicSource.Play();
        }

    }

    public void PlaySFX(string name){
        Sound s = System.Array.Find(sfxSounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("Sound: " + name + " not found!");
            return;
        }
        else{
            sfxSource.PlayOneShot(s.clip);
        }
    }

    // USE THIS CODE FOR SOUND PANEL:

    public voice ToggleMusic(){
        musicSource.mute = !musicSource.mute;
    }
    public voice ToggleSFX(){
        sfxSource.mute = !sfxSource.mute;
    }

    public void MusicVolume(float volume){
        musicSource.volume = volume;
    }

    public void SFXVolume(float volume){
        sfxSource.volume = volume;
    }

}
