using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/*
 * https://www.youtube.com/watch?v=qXbjyzBlduY&ab_channel=SasquatchBStudios
 */

public class ResetDeviceBindings : MonoBehaviour
{
    [SerializeField] InputActionAsset inputActions;
    [SerializeField] string targetControlScheme;

    public void ResetControlSchemeBindings()
    {
        foreach(InputActionMap map in inputActions.actionMaps)
        {
            foreach(InputAction action in map)
            {
                action.RemoveBindingOverride(InputBinding.MaskByGroup(targetControlScheme));
            }
        }
        PlayerPrefs.DeleteKey("rebinds");
    }
}
