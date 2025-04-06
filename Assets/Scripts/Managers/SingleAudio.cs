using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Same as AudioManager but for individual game objects
Basically, if you do not want the component to persist between scenes
*/

public class SingleAudio : MonoBehaviour
{
    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource;

    public void PlayMusic(string name, bool _loop = false)
    {
        Sound s = System.Array.Find(musicSounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("Sound: " + name + " not found!");
            return;
        }
        else
        {
            musicSource.clip = s.clip;
            musicSource.loop = _loop;
            musicSource.Play();
        }

    }

    public void PlaySFX(string name, bool loop = false)
    {
        Sound s = System.Array.Find(sfxSounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("Sound: " + name + " not found!");
            return;
        }
        else
        {
            sfxSource.clip = s.clip;
            sfxSource.loop = loop;
            sfxSource.Play();
        }
    }

    public void StopSFX()
    {
        sfxSource.Stop();
        sfxSource.loop = false;
        sfxSource.clip = null; // Force reset
    }

    // USE THIS CODE FOR SOUND PANEL:

    public void ToggleMusic()
    {
        musicSource.mute = !musicSource.mute;
    }
    public void ToggleSFX()
    {
        sfxSource.mute = !sfxSource.mute;
    }

    public void MusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void SFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }

}
