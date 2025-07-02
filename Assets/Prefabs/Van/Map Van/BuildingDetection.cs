using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*
 * Detection collider attached to Map Van
 * If collided with building collider:
 *    Show a canvas that display's building info
 * If no longer colliding:
 *    Destroy canvas
 */

public class BuildingDetection : MonoBehaviour
{
    [SerializeField] GameObject playerCam;

    List<GameObject> buildingsDetected = new List<GameObject>();
    GameObject selectedBuilding;

    private void Update()
    {
        // Interact and selected building exists
        if (((UserInput.Instance && UserInput.Instance.Interact) || (UserInput.Instance == null && Input.GetMouseButtonDown(0))) && selectedBuilding)
        {
            ExecuteEvents.Execute<InteractEvent>(selectedBuilding, null, (x, y) => x.Interact());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Wall")) {
            buildingsDetected.Add(other.gameObject);
            FindClosestBuilding();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Wall"))
        {
            BaseCanvasType buildingInfo = other.GetComponentInChildren<BaseCanvasType>();
            if (buildingInfo)
            {
                buildingsDetected.Remove(other.gameObject);
                Destroy(buildingInfo.gameObject);
            }
            FindClosestBuilding();
        }
    }

    /*
     * Get distance of collider
     * if one is closer, switch to that one
     * on interact, go to current selection
    */
    void FindClosestBuilding()
    {
        if(buildingsDetected.Count > 0)
        {
            float closestDist = Mathf.Infinity;
            GameObject newSelection = null;
            for (int i = 0; i < buildingsDetected.Count; i++)
            {
                float distance = Vector3.Distance(transform.position, buildingsDetected[i].transform.position);
                if (distance < closestDist)
                {
                    newSelection = buildingsDetected[i].gameObject;
                    closestDist = distance;
                }
            }

            // spawn building canvas but only if it is a different building
            if (newSelection != null && newSelection != selectedBuilding)
            {
                if(selectedBuilding)  // destroy previous selected canvas
                    DestroyBuildingCanvas(selectedBuilding);

                selectedBuilding = newSelection;
                selectedBuilding.GetComponent<BaseMapBuilding>().CreateCanvas(playerCam);
            }
        }
        else
        {
            selectedBuilding = null;
        }
    }

    private void DestroyBuildingCanvas(GameObject building)
    {
        BaseCanvasType infoCanvas = building.GetComponentInChildren<BaseCanvasType>();
        if (infoCanvas)
            Destroy(infoCanvas.gameObject);
    }

}
