using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickToPlaySound : MonoBehaviour, InteractEvent
{
    [SerializeField] string sound;
    [SerializeField] SingleAudio singleAudio;

    public void Interact()
    {
        singleAudio.PlaySFX(sound);
    }
}
