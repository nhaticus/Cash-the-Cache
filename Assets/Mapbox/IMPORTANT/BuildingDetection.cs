using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    BuildingInfo selectedBuilding;
    bool inShop = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (selectedBuilding)
            {
                PlayerPrefs.SetInt("Difficulty", selectedBuilding.difficulty);
                SceneManager.LoadScene(buildingLevelScene);
            }
            else if (inShop)
            {
                SceneManager.LoadScene(shopScene);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Wall") {
            BuildingInfo info = other.GetComponentInChildren<BuildingInfo>();
            if (info)
            {
                // create a canvas at object position
                Vector3 pos = other.GetComponentInChildren<BuildingInfo>().gameObject.transform.position;
                pos.y += 10;
                GameObject infoCanvas = Instantiate(infoCanvasPrefab, pos, Quaternion.identity);
                infoCanvas.GetComponent<BuildingInfoCanvas>().buildingInfo = info;
                infoCanvas.GetComponent<BuildingInfoCanvas>().playerCameraTransform = playerCam.transform;
                infoCanvas.transform.SetParent(other.transform);

                selectedBuilding = info;
            }
        }
        else if(other.tag == "Door") // Dumb: fix later
        {
            inShop = true;
            // create a canvas at object position
            Vector3 pos = other.GetComponentInChildren<ShopInfo>().gameObject.transform.position;
            pos.y += 10;
            GameObject shopCanvas = Instantiate(shopCanvasPrefab, pos, Quaternion.identity);
            shopCanvas.GetComponent<BuildingInfoCanvas>().playerCameraTransform = playerCam.transform;
            shopCanvas.transform.SetParent(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Wall")
        {
            // delete info canvas
            BuildingInfoCanvas infoCanvas = other.GetComponentInChildren<BuildingInfoCanvas>();
            Destroy(infoCanvas.gameObject);
        }
        else if (other.tag == "Door")
        {
            inShop = false;
        }
    }
}
