using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/*
 * https://www.youtube.com/watch?v=7t218LaeiiI&ab_channel=AllThingsGameDev
 */

public class Door : MonoBehaviour, InteractEvent
{
    [SerializeField] GameObject door;
    [SerializeField] float openRot, closeRot;
    [SerializeField] float speed;
    [SerializeField] bool opening;

    void Update()
    {
        Vector3 currentRot = door.transform.localEulerAngles;
        if (opening)
        {
            if(currentRot.y < openRot)
            {
                door.transform.localEulerAngles = Vector3.Lerp(currentRot, new Vector3(currentRot.x, openRot, currentRot.z), speed * Time.deltaTime);
            }
        }
        else // closing
        {
            if (currentRot.y > closeRot)
            {
                door.transform.localEulerAngles = Vector3.Lerp(currentRot, new Vector3(currentRot.x, closeRot, currentRot.z), speed * Time.deltaTime);
            }
        }
    }
    public void Interact()
    {
        opening = !opening;
    }
}
