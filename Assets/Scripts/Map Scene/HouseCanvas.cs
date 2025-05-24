using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class HouseCanvas : BaseCanvasType
{
    public int difficulty = 0;
    [SerializeField] TMP_Text difficultyText;

    [SerializeField] LocalizedString difficultyLocalizedString;

    private void Start()
    {
        UpdateDifficultyText();
        StartCoroutine(StartUp());
    }

    public void UpdateDifficultyText()
    {
        difficultyLocalizedString.StringChanged -= UpdateText;

        difficultyLocalizedString.Arguments = new object[] { difficulty };
        difficultyLocalizedString.StringChanged += UpdateText;
        difficultyLocalizedString.RefreshString();
    }


    private void UpdateText(string localizedText)
    {
        difficultyText.text = localizedText;
    }

    private void OnDisable()
    {
        difficultyLocalizedString.StringChanged -= UpdateText;
    }
}