using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseCanvas : BaseCanvasType
{
    public int difficulty = 0;
    int totalDifficulty = 5;
    [SerializeField] GameObject[] stars;

    private void Start()
    {
        CreateDifficultyStars();
        StartCoroutine(StartUp());
    }

    void CreateDifficultyStars()
    {
        for (int i = 0; i < totalDifficulty; i++)
        {
            if (i <= difficulty)
            { // filled in star
                stars[i].GetComponent<Animator>().SetBool("Full", true);
            }
        }
    }
}