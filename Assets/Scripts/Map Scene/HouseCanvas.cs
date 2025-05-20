using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HouseCanvas : BaseCanvasType
{
    public int difficulty = 0;
    [SerializeField] TMP_Text difficultyText;

    private void Start()
    {
        difficultyText.text = "Difficulty: " + difficulty.ToString();
        StartCoroutine(StartUp());
    }
}