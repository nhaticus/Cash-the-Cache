using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Base class for canvas' owned by Map Buildings
 */

public abstract class BaseCanvasType : MonoBehaviour
{
    [HideInInspector] public Transform playerCameraTransform; // maybe look at inbetween camera and van position
    float totalWidthTime = 0.2f, totalHeightTime = 0.2f;
    float startingHeight = 0.3f;

    private void Start()
    {
        StartCoroutine(StartUp());
    }

    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - playerCameraTransform.position);
    }
    public IEnumerator StartUp()
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
