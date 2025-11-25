using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Create raycast by finding correct position and rotation
 * Mostly relies on RotateAroundPivot()
 * 
 * 1. Find needed rotation
 * 2. Get the position of the selected door when RotateAroundPivot()
 * 3. Get the difference between selected door and door to connect to
 * 4. Get needed position by adding difference and rotated box center
 * 5. DrawBox()
 */

public class RoomLookAt : MonoBehaviour
{
    [Header("ToSpawn room")]
    [SerializeField] GameObject toSpawnRoom;

    [Header("Dummy")]
    [SerializeField] Transform dummy;

    [Header("Room to connect to")]
    [SerializeField] Transform roomConnectTo;
    [SerializeField] Transform connectPoint;

    void Start()
    {
        Bleh(toSpawnRoom);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Bleh(toSpawnRoom);
        }
    }

    Vector3 newRoomPosition = Vector3.zero;
    Quaternion orientation = Quaternion.identity;
    Vector3 boxSize;
    void Bleh(GameObject toSpawnRoom)
    {
        dummy.position = Vector3.zero;
        RoomInfo roomInfo = toSpawnRoom.GetComponent<RoomInfo>();
        Transform selectedDoor = roomInfo.doorPoints[Random.Range(0, roomInfo.doorPoints.Length)];
        Debug.Log("selected door: " + selectedDoor.parent.name);

        // find needed rotation
        float needRot = connectPoint.rotation.eulerAngles.y - selectedDoor.rotation.eulerAngles.y - 180;
        // Debug.Log("connect euler: " + connectPoint.rotation.eulerAngles.y + "  select: " + selectedDoor.rotation.eulerAngles.y);
        Debug.Log("rotate to: " + new Vector3(0, needRot, 0));
        dummy.eulerAngles = new Vector3(0, needRot, 0);

        // get position of RotatePivot(door)
        toSpawnRoom.transform.position = Vector3.zero;
        Vector3 rotatedDoorPos = RotatePointAroundPivot(selectedDoor.position, toSpawnRoom.transform.position, new Vector3(0, needRot, 0));

        // get difference between Rotated door and door to connect to
        Vector3 difference = (rotatedDoorPos - connectPoint.position) * -1;
        dummy.position = difference;

        // draw box stuff
        BoxCollider box = toSpawnRoom.GetComponent<BoxCollider>();
        Vector3 rotBoxCenter = RotatePointAroundPivot(box.center, toSpawnRoom.transform.position, new Vector3(0, needRot, 0));
        newRoomPosition = difference + rotBoxCenter;
        orientation = Quaternion.Euler(new Vector3(0, needRot, 0));
        boxSize = box.size;
    }

    private void OnDrawGizmos()
    {
        DrawBox(newRoomPosition, orientation, boxSize, Color.red);
    }

    private Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        return Quaternion.Euler(angles) * (point - pivot) + pivot;
    }

    public void DrawBox(Vector3 pos, Quaternion rot, Vector3 scale, Color c)
    {
        // create matrix
        Matrix4x4 m = new Matrix4x4();
        m.SetTRS(pos, rot, scale);

        var point1 = m.MultiplyPoint(new Vector3(-0.5f, -0.5f, 0.5f));
        var point2 = m.MultiplyPoint(new Vector3(0.5f, -0.5f, 0.5f));
        var point3 = m.MultiplyPoint(new Vector3(0.5f, -0.5f, -0.5f));
        var point4 = m.MultiplyPoint(new Vector3(-0.5f, -0.5f, -0.5f));

        var point5 = m.MultiplyPoint(new Vector3(-0.5f, 0.5f, 0.5f));
        var point6 = m.MultiplyPoint(new Vector3(0.5f, 0.5f, 0.5f));
        var point7 = m.MultiplyPoint(new Vector3(0.5f, 0.5f, -0.5f));
        var point8 = m.MultiplyPoint(new Vector3(-0.5f, 0.5f, -0.5f));

        Debug.DrawLine(point1, point2, c);
        Debug.DrawLine(point2, point3, c);
        Debug.DrawLine(point3, point4, c);
        Debug.DrawLine(point4, point1, c);

        Debug.DrawLine(point5, point6, c);
        Debug.DrawLine(point6, point7, c);
        Debug.DrawLine(point7, point8, c);
        Debug.DrawLine(point8, point5, c);

        Debug.DrawLine(point1, point5, c);
        Debug.DrawLine(point2, point6, c);
        Debug.DrawLine(point3, point7, c);
        Debug.DrawLine(point4, point8, c);
    }
}