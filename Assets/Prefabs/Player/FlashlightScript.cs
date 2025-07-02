using UnityEngine;

public class FlashlightScript : MonoBehaviour
{
    [SerializeField] PlayerCam playerCam;
    [SerializeField] GameObject lightObj;

    void Start()
    {
        lightObj.SetActive(PlayerManager.Instance.hasFlashlight);
    }

    private void Update()
    {
        if (!playerCam.lockRotation) //rotates flashlight based on camera rotation
        {
            transform.rotation = Quaternion.Euler(playerCam.xRotation, playerCam.yRotation, 0);
        }

        if (Input.GetKeyDown(KeyCode.F) && PlayerManager.Instance.hasFlashlight)
        {
            lightObj.SetActive(!lightObj.activeSelf);
        }
    }

}
