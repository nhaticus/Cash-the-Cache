using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


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

    void OnEnable() {
        SceneManager.sceneLoaded += OnSceneChanged;
    }

    void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneChanged;
    }

    private void OnSceneChanged(Scene scene, LoadSceneMode mode){
        if(scene.name == "Shop"){
            PlayMusic("shop_music");
        }
        else {
            PlayMusic("idle_music");
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

    public void StopSFX(string name){
        //Debug.Log($"Attempting to stop: {name}");
        if (sfxSource.clip != null) {
            //Debug.Log($"Current clip name: {sfxSource.clip.name}");
        } else {
            Debug.Log("No clip assigned to sfxSource");
        }

        //Debug.Log("Stopping SFX: " + name);
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
