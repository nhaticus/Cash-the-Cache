using System.Collections;
using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.Rendering;

/*
Same as AudioManager but for individual game objects
Basically, if you do not want the component to persist between scenes
*/

public class SingleAudio : MonoBehaviour
{
    public Sound[] musicSounds, sfxSounds;
    public AudioSource[] musicSources, sfxSources;

    public void PlayMusic(string name = "", bool _loop = false, AudioSource source = null)
    {
        Sound s = System.Array.Find(musicSounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("Sound: " + name + " not found!");
        }
        else
        {
            AudioSource validSource;
            if (source)
                validSource = source;
            else
                validSource = GetAnyMusicSource();

            validSource.clip = s.clip;
            validSource.loop = _loop;
            validSource.Play();
        }

    }
    public void PlaySFX(string name = "", bool loop = false, AudioSource source = null)
    {
        Sound s = System.Array.Find(sfxSounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("Sound: " + name + " not found!");
        }
        else
        {
            AudioSource validSource;
            if (source)
                validSource = source;
            else
                validSource = GetAnySFXSource();
                

            validSource.clip = s.clip;
            validSource.loop = loop;
            validSource.Play();
        }
    }

    public AudioSource PickUnusedMusicSource()
    {
        // loop through musicSource, find unused source, and play music in that source
        AudioSource validSource = null;
        foreach (AudioSource source in musicSources)
        {
            if (!source.isPlaying)
            {
                validSource = source;
                break;
            }
        }

        return validSource;
    }

    public AudioSource PickUnusedSFXSource()
    {
        // loop through musicSource, find unused source, and play music in that source
        AudioSource validSource = null;
        foreach (AudioSource source in sfxSources)
        {
            if (!source.isPlaying)
            {
                validSource = source;
                break;
            }
        }

        return validSource;
    }

    /// <summary>
    /// Tries to get an unused source first
    /// If not then takes first source
    /// </summary>
    /// <returns></returns>
    public AudioSource GetAnyMusicSource()
    {
        AudioSource source = PickUnusedMusicSource();
        if (source == null)
            source = musicSources[0];

        return source;
    }

    public AudioSource GetAnySFXSource()
    {
        AudioSource source = PickUnusedSFXSource();
        if(source == null)
            source = sfxSources[0];

        return source;
    }

    /// <summary>
    /// Find source that is playing sound
    /// </summary>
    /// <param name="name"></param>
    public AudioSource FindSource(string name, AudioSource[] sources)
    {
        foreach (AudioSource source in sources)
        {
            if (source.clip.name == name)
            {
                return source;
            }  
        }
        return null; // none found
    }

    public void StopSelectSFX(string name, AudioSource source = null)
    {
        // check if source is given
        AudioSource chosenSource;
        if (source)
            chosenSource = source;
        else
            chosenSource = FindSource(name, sfxSources);

        if (chosenSource)
        {
            chosenSource.Stop();
            chosenSource.loop = false;
            chosenSource.clip = null; // Force reset
        }
    }

    public void StopAllSFX()
    {
        // stop all sfx
        foreach (AudioSource source in musicSources)
        {
            source.Stop();
            source.loop = false;
            source.clip = null; // Force reset
        }
    }

    /*   ---- SOUND PANEL ----   */

    public void ToggleMusic()
    {
        // toggle all music sources
        foreach (AudioSource source in musicSources)
        {
            source.mute = !source.mute;
        }
    }
    public void ToggleSFX()
    {
        foreach (AudioSource source in sfxSources)
        {
            source.mute = !source.mute;
        }
    }

    public void MusicVolume(float volume)
    {
        foreach (AudioSource source in musicSources)
        {
            source.volume = volume;
        }
    }

    public void SFXVolume(float volume)
    {
        foreach (AudioSource source in sfxSources)
        {
            source.volume = volume;
        }
    }

}
