using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMusic : MonoBehaviour
{
    [SerializeField] SingleAudio singleAudio;
    [SerializeField] string musicName = "music";
    void Start()
    {
        singleAudio.PlayMusic(musicName, _loop: true);
    }
}
