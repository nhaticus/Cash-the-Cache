using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomImageOnStart : MonoBehaviour
{
    [SerializeField] Sprite[] sprites;
    [SerializeField] Image imageToChange;

    void Start()
    {
        imageToChange.sprite = sprites[Random.Range(0, sprites.Length - 1)];
    }

}
