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
        gameObject.tag = "Wall";
    }
    public abstract void CreateCanvas(GameObject playerCam);

    public GameObject CreateFollowingCanvas(GameObject playerCam)
    {
        Vector3 pos = transform.position;
        pos.y += spawnY;
        GameObject infoCanvas = Instantiate(canvas, pos, Quaternion.identity);
        infoCanvas.GetComponent<BaseCanvasType>().playerCameraTransform = playerCam.transform;
        infoCanvas.transform.SetParent(gameObject.transform);
        return infoCanvas;
    }

    public abstract void Interact();
}
