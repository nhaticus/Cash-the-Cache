using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * set rotation = (connect's room rotation + connect door rotation + connect point rotation)
 * - (select door rotation + select door point rotation) - 180
 * 
 * OR
 * 
 * connect's point rot - select point rot - 180
 */

public class RoomLookAt : MonoBehaviour
{
    [Header("Spawned room")]
    [SerializeField] Transform room;
    [SerializeField] Transform door;
    [SerializeField] Transform doorPoint;

    [Header("Room to connect to")]
    [SerializeField] Transform roomConnectTo;
    [SerializeField] Transform doorConnectTo;
    [SerializeField] Transform connectPoint;

    // works with some doors to some doors

    void Start()
    {
        //rotate
        float setRot = connectPoint.rotation.eulerAngles.y - doorPoint.rotation.eulerAngles.y - 180;
        
        room.eulerAngles = new Vector3(0, setRot, 0);

        Vector3 horizontalCurrentForward = new Vector3(doorConnectTo.forward.x, 0, doorConnectTo.forward.z).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(horizontalCurrentForward);
        Quaternion newRoomRotation = targetRotation * Quaternion.Inverse(door.rotation);


        // move
        Vector3 doorOffset = doorPoint.position - room.position;
        Debug.Log(connectPoint.position + " - " + newRoomRotation + " * " + doorOffset);
        Vector3 diff = newRoomRotation * doorOffset;
        Debug.Log(diff);
        Vector3 something = connectPoint.position + diff;
        Debug.Log(something);
        Vector3 newRoomPosition = connectPoint.position - newRoomRotation * doorOffset; // get current door's real world position
        
        room.position = newRoomPosition;
        Debug.Log("new pos: " + newRoomPosition);
    }
}
