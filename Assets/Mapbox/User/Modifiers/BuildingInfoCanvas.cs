using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/*
 * Canvas that displays Building Info
 */

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
            floorsText.text = "Floors: " + buildingInfo.floors;
        }

        StartCoroutine(StartUp());
    }
    
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - playerCameraTransform.position);
    }

    IEnumerator StartUp()
    {
        transform.localScale = new Vector3(0, 0.3f, 1);
        float totalWidthTime = 0.3f;
        float timeTaken = 0;
        while (timeTaken < totalWidthTime)
        {
            transform.localScale = new Vector3(timeTaken / totalWidthTime, 0.3f, 1);
            timeTaken += Time.deltaTime;
            yield return null;
        }

        timeTaken = 0;
        float totalHeightTime = 0.3f;
        while (timeTaken < totalHeightTime)
        {
            float height = timeTaken / totalHeightTime - 0.3f;
            if (height < 0.3f)
                height = 0.3f;
            transform.localScale = new Vector3(transform.localScale.x, 0.3f + height, 1);
            timeTaken += Time.deltaTime;
            yield return null;
        }
    }
    
    
}
