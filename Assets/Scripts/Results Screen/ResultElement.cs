using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultElement : MonoBehaviour
{
    [SerializeField] Image img;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text amtValText;
    [SerializeField] TMP_Text totalText;

    public void Initialize(Sprite _sprite, string _name, int amt, int value)
    {
        img.sprite = _sprite;
        nameText.text = _name;
        amtValText.text = amt + " X " + value;
        totalText.text = ": " + (amt * value).ToString();
    }
}
