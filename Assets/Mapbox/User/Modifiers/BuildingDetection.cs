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
    [SerializeField] GameObject infoCanvasPrefab;
    [SerializeField] GameObject playerCam;

    BuildingInfo selectedBuilding;

    private void Update()
    {
        if (selectedBuilding && Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("Main Level");
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
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Wall")
        {
            // delete info canvas
            BuildingInfoCanvas infoCanvas = other.GetComponentInChildren<BuildingInfoCanvas>();
            Destroy(infoCanvas.gameObject);
        }
    }
}
