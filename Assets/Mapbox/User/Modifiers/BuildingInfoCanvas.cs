using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuildingInfoCanvas : MonoBehaviour
{
    public Transform playerCameraTransform; // maybe look at inbetween camera and van position
    public BuildingInfo buildingInfo;

    [SerializeField] TMP_Text difficultyText, floorsText;

    private void Start()
    {
        if (buildingInfo)
        {
            difficultyText.text = "Difficulty: " + buildingInfo.difficulty;
            floorsText.text = "Difficulty: " + buildingInfo.floors;
        }
    }

    void Update()
    {
        transform.LookAt(playerCameraTransform);
    }
}
