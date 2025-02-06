using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultElement : MonoBehaviour
{
    [SerializeField] Image img;
    [SerializeField] TMP_Text name;
    [SerializeField] TMP_Text amtValText;
    [SerializeField] TMP_Text totalText;

    public void Initialize(Sprite _sprite, string _name, string amtVal, string total)
    {
        img.sprite = _sprite;
        name.text = _name;
        amtValText.text = amtVal;
        totalText.text = total;
    }
}
