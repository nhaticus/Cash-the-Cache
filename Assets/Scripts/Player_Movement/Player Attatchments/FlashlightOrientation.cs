using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightOrientation : MonoBehaviour
{
    public PlayerCam playerCam;

    private void Update()
    {
        if (!playerCam.lockRotation)
        {
            //rotates flashlight based on camera rotation
            transform.rotation = Quaternion.Euler(playerCam.xRotation, playerCam.yRotation, 0);
        }
    }
}
