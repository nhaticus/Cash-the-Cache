using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Used for starting music when game is finished loading instead of during a loading screen
 */

public class GameMusic : MonoBehaviour
{
    [SerializeField] SingleAudio singleAudio;
    [SerializeField] string musicName = "music";

    [SerializeField] bool onStart = true; // if you want music to start on scene load, otherwise you can start with BeginMusic
    
    
    void Start()
    {
        if(onStart)
            BeginMusic();
    }

    public void BeginMusic()
    {
        singleAudio.PlayMusic(musicName, _loop: true);
    }
}
