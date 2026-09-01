using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class ResultElement : MonoBehaviour
{
    [SerializeField] CanvasGroup container;
    [SerializeField] Image img;
    [SerializeField] TextMeshProUGUI textDisplay;
    LocalizedString stringReference;
    [SerializeField] TMP_Text amtValText;
    [SerializeField] TMP_Text totalText;

    public event Action OnTranslationFinished;

    public void Initialize(Sprite _sprite, LocalizedString _name, int amt, int value)
    {
        if(container != null)
            container.alpha = 0;

        img.sprite = _sprite;

        stringReference = _name;

        amtValText.text = amt + " X " + value;
        totalText.text = ": " + (amt * value).ToString();

        stringReference.StringChanged += OnReady;
    }

    void OnReady(string translatedText)
    {
        if (textDisplay != null)
            textDisplay.text = translatedText;

        if (container != null)
            container.alpha = 1f;

        // send signal to ResultScreen to create next ResultElement
        OnTranslationFinished?.Invoke();
    }
}
