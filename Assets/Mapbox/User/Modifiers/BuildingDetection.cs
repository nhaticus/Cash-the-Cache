using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Wall") {
            BuildingInfo info = other.GetComponentInChildren<BuildingInfo>();
            if (info)
            {
                // create a canvas at object position
                Vector3 pos = other.GetComponentInChildren<BuildingInfo>().gameObject.transform.position;
                pos.y += 14;
                GameObject infoCanvas = Instantiate(infoCanvasPrefab, pos, Quaternion.identity);
                infoCanvas.GetComponent<BuildingInfoCanvas>().buildingInfo = info;
                infoCanvas.GetComponent<BuildingInfoCanvas>().playerCameraTransform = playerCam.transform;
                infoCanvas.transform.SetParent(other.transform);
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
