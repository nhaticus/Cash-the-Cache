using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


/*
HOW TO USE SFX:
upon collision/movement call: AudioManager.Instance.PlaySFX("name_of_clip");
HOW TO STOP IDLE MUSIC:
upon start game, etc. call: AudioManager.Instance.musicSource.Stop();
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
        musicSource.volume = PlayerPrefs.GetFloat("Music");
        sfxSource.volume = PlayerPrefs.GetFloat("SFX");
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneChanged;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneChanged;
    }

    private void OnSceneChanged(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Shop")
        {
            musicSource.volume = 0.1f;
            PlayMusic("shop_music");
        }
        else if (scene.name == "Main Level")
        {
            // Reduce volume by 20%
            PlayMusic("level_music");
        }
        else
        {
            // :)
            PlayMusic("menu_music");
        }
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

    public void PlaySFX(string name, bool loop=false){
        Sound s = System.Array.Find(sfxSounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("Sound: " + name + " not found!");
            return;
        }

        if (loop) {
            sfxSource.clip = s.clip;
            sfxSource.loop = true;
            sfxSource.Play();
        } else {
            sfxSource.clip = s.clip;
            sfxSource.loop = false;
            sfxSource.Play();
        }
    }

    public void PlaySFXOneShot(string name)
    {
        Sound s = System.Array.Find(sfxSounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("Sound: " + name + " not found!");
            return;
        }
        Debug.Log("Playing SFX: " + name);
        sfxSource.PlayOneShot(s.clip);
    }


    public void StopSFX(){
        sfxSource.Stop();
        sfxSource.loop = false;
        sfxSource.clip = null; // Force reset

    }

    // USE THIS CODE FOR SOUND PANEL:

    public void ToggleMusic(){
        musicSource.mute = !musicSource.mute;
    }
    public void ToggleSFX(){
        sfxSource.mute = !sfxSource.mute;
    }

    public void MusicVolume(float volume){
        musicSource.volume = volume;
    }

    public void SFXVolume(float volume){
        sfxSource.volume = volume;
    }

}
