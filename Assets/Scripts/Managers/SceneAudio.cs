using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneAudio : MonoBehaviour
{
    [SerializeField] SingleAudio singleAudio;
    [SerializeField] string musicName;

    private void Start()
    {
        singleAudio.PlayMusic(musicName);
        singleAudio.musicSource.loop = true;
    }
}
