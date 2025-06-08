using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMusic : MonoBehaviour
{
    [SerializeField] SingleAudio singleAudio;
    [SerializeField] string musicName = "music";

    [SerializeField] bool onStart = true; // if you want music to start on scene load, otherwise you can start with BeginMusic
    // Just used for starting music when game is finished loading instead of loading screen
    
    void Start()
    {
        if(onStart)
            BeginMusic();
    }

    // just here for loading
    public void BeginMusic()
    {
        singleAudio.PlayMusic(musicName, _loop: true);
    }
}
