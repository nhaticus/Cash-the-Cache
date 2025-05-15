using System.Collections;
using TMPro;
using UnityEngine;

/*
 * Canvas that displays Building Info
 */

public class BuildingInfoCanvas : MonoBehaviour
{
    public Transform playerCameraTransform; // maybe look at inbetween camera and van position
    public BuildingInfo buildingInfo;

    [SerializeField] TMP_Text difficultyText;
    float totalWidthTime = 0.2f, totalHeightTime = 0.2f;
    float startingHeight = 0.3f;

    private void Start()
    {
        if (buildingInfo)
        {
            difficultyText.text = "Difficulty: " + buildingInfo.difficulty.ToString();
        }

        StartCoroutine(StartUp());
    }
    
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - playerCameraTransform.position);
    }

    IEnumerator StartUp()
    {
        // stretch horizontally
        transform.localScale = new Vector3(0, startingHeight, 1);
        float timeTaken = 0;
        while (timeTaken <= totalWidthTime)
        {
            transform.localScale = new Vector3(timeTaken / totalWidthTime, startingHeight, 1);
            timeTaken += Time.deltaTime;
            yield return null;
        }
        transform.localScale = new Vector3(1, startingHeight, 1); // forcing it to be correct scale

        // stretch vertically
        timeTaken = 0;
        while (timeTaken <= totalHeightTime)
        {
            float height = ((timeTaken / totalHeightTime) * (1 - startingHeight)) + startingHeight;
            transform.localScale = new Vector3(transform.localScale.x, height, 1);
            timeTaken += Time.deltaTime;
            yield return null;
        }
        transform.localScale = new Vector3(1, 1, 1); // forcing it to be correct scale
    }
    
    
}
