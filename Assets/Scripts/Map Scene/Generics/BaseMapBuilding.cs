using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/*
 * A base class for Map buildings
 */

public abstract class BaseMapBuilding : MonoBehaviour, InteractEvent
{
    [SerializeField] GameObject canvas;
    float spawnY = 5;

    private void Awake()
    {
        // Set parent building object tag to Wall so an Building Detection can find it
        gameObject.tag = "Wall";
    }
    public abstract void CreateCanvas(GameObject playerCam);

    public GameObject CreateFollowingCanvas(GameObject playerCam)
    {
        // Create Canvas
        Vector3 pos = transform.position;
        pos.y += spawnY;
        GameObject infoCanvas = Instantiate(canvas, pos, Quaternion.identity);
        infoCanvas.GetComponent<BaseCanvasType>().playerCameraTransform = playerCam.transform;

        // Parent canvas under building
        infoCanvas.transform.SetParent(gameObject.transform);

        return infoCanvas;
    }

    public abstract void Interact();
}
