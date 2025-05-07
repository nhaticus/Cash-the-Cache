using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * Detection collider attached to escape sequence Van
 * If collided with building collider:
 *    Show a canvas that display's building info
 * If no longer colliding:
 *    Destroy canvas
 */

public class BuildingDetection : MonoBehaviour
{
    [SerializeField] GameObject infoCanvasPrefab, shopCanvasPrefab;
    [SerializeField] GameObject playerCam;
    [SerializeField] string buildingLevelScene = "Level Gen";
    [SerializeField] string shopScene = "Shop";

    List<BuildingInfo> buildingsDetected = new List<BuildingInfo>();
    GameObject selectedBuilding;
    bool inShop = false;

    private void Update()
    {
        FindClosestBuilding();

        if ((UserInput.Instance && UserInput.Instance.Interact) || (UserInput.Instance == null && Input.GetMouseButtonDown(0)))
        {
            if (inShop)
            {
                SceneManager.LoadScene(shopScene);
            }
            else if (selectedBuilding)
            {
                PlayerPrefs.SetInt("Difficulty", selectedBuilding.GetComponentInChildren<BuildingInfo>().difficulty);
                SceneManager.LoadScene(buildingLevelScene);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Wall") {
            buildingsDetected.Add(other.GetComponentInChildren<BuildingInfo>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Wall")
        {
            BuildingInfo buildingInfo = other.GetComponentInChildren<BuildingInfo>();
            if (buildingInfo)
            {
                buildingsDetected.Remove(buildingInfo);
                DestroyInfoCanvas(other.gameObject);

                if (buildingInfo.isShop)
                    inShop = false;
            }
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
                    DestroyInfoCanvas(selectedBuilding);

                selectedBuilding = newSelection;

                if (selectedBuilding.GetComponentInChildren<BuildingInfo>().isShop)
                {
                    inShop = true;
                    CreateShopCanvas(selectedBuilding);
                }
                else
                {
                    CreateBuildingCanvas(selectedBuilding);
                }
            }
        }
        else
        {
            selectedBuilding = null;
        }
    }

    void CreateBuildingCanvas(GameObject building)
    {
        BuildingInfo info = building.GetComponentInChildren<BuildingInfo>();
        Vector3 pos = info.gameObject.transform.position;
        pos.y += 10;
        GameObject infoCanvas = Instantiate(infoCanvasPrefab, pos, Quaternion.identity);
        if(info)
            infoCanvas.GetComponent<BuildingInfoCanvas>().buildingInfo = info;
        infoCanvas.GetComponent<BuildingInfoCanvas>().playerCameraTransform = playerCam.transform;
        infoCanvas.transform.SetParent(building.transform);
    }

    void CreateShopCanvas(GameObject shop)
    {
        Vector3 pos = shop.GetComponentInChildren<BuildingInfo>().gameObject.transform.position;
        pos.y += 10;
        GameObject shopCanvas = Instantiate(shopCanvasPrefab, pos, Quaternion.identity);
        shopCanvas.GetComponent<BuildingInfoCanvas>().playerCameraTransform = playerCam.transform;
        shopCanvas.transform.SetParent(shop.transform);
    }

    void DestroyInfoCanvas(GameObject building)
    {
        BuildingInfoCanvas infoCanvas = building.GetComponentInChildren<BuildingInfoCanvas>();
        if (infoCanvas)
            Destroy(infoCanvas.gameObject);
    }

}
