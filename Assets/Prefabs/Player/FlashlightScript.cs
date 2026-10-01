using UnityEngine;

public class FlashlightScript : MonoBehaviour
{
    [SerializeField] PlayerCam playerCam;
    [SerializeField] GameObject lightObj;

    bool hasFlashlight = false;

    void Start()
    {
        hasFlashlight = DataSystem.GetOrCreateItem("Flashlight").level == 1;
        lightObj.SetActive(hasFlashlight);
    }

    private void Update()
    {
        if (!playerCam.lockRotation) //rotates flashlight based on camera rotation
        {
            transform.rotation = Quaternion.Euler(playerCam.xRotation, playerCam.yRotation, 0);
        }
    }

}
