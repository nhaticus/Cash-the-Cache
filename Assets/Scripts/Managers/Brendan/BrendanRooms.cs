using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.UIElements;

public class BrendanRooms : MonoBehaviour
{
    [Header("Setup (optional)")]
    public GameObject[] startRooms;
    public int maxRooms = 7;
    public int minRooms = 5;
    public int maxRetries = 30;
    public NavMeshSurface surface;

    [Header("House Rooms")]
    public GameObject[] roomPrefabs; // list of all rooms that can be spawned

    Queue<Transform> availableDoors = new Queue<Transform>(); // queue of all doors available to spawn from
    List<GameObject> placedRooms = new List<GameObject>(); // list of all placed rooms

    int roomCount = 0;
    int retryNum = 0;
    Vector3 levelSpawnPosition;
    Quaternion levelSpawnRotation;

    // send event when enough Rooms Generated
    public UnityEvent roomsFinished;

    void Start()
    {
        SetDifficulty();
        BuildHouse();
    }

    void SetDifficulty()
    {
        // increase number of rooms based on number of plays and difficulty
        minRooms += (int) Mathf.Floor(1.12f * (DataSystem.Data.gameState.currentReplay / 10) + PlayerPrefs.GetInt("Difficulty") / 1.5f);
        maxRooms += (int) Mathf.Floor(1.1f * (DataSystem.Data.gameState.currentReplay / 10) + PlayerPrefs.GetInt("Difficulty") / 1.3f);
    }

    public void BuildHouse()
    {
        levelSpawnPosition = transform.position;
        levelSpawnRotation = transform.rotation;

        CreateStartRoom();

        StartCoroutine(GenerateRooms());
    }

    void CreateStartRoom()
    {
        GameObject startRoomPrefab;
        if (startRooms.Length > 0) // take from startRooms
            startRoomPrefab = startRooms[Random.Range(0, startRooms.Length)];
        else // take from any room
            startRoomPrefab = roomPrefabs[Random.Range(0, roomPrefabs.Length)];

        GameObject startRoom = Instantiate(startRoomPrefab, levelSpawnPosition, levelSpawnRotation);
        startRoom.transform.SetParent(transform);
        placedRooms.Add(startRoom);
        roomCount++;
        RoomInfo startRoomScript = startRoom.GetComponent<RoomInfo>();
        if (startRoomScript != null)
        {
            foreach(Transform door in startRoomScript.doorPoints)
            {
                availableDoors.Enqueue(door);
            }
        }
    }

    /// <summary>
    /// IEnumerator that determines which rooms to spawn and creates them all.<br />
    /// IEnumerator because it needs to be able to run without freezing the game to generate rooms.<br />
    /// Must be called after a room has already been created, because it expects doors from the queue
    /// </summary>
    IEnumerator GenerateRooms()
    {
        yield return new WaitForSeconds(0.3f);

        if (retryNum > maxRetries)
        {
            minRooms--;
            retryNum = 0;
        }

        // while there are doors to add rooms to
        // and haven't hit max rooms yet
        while (availableDoors.Count > 0 && roomCount < maxRooms)
        {
            Transform doorToConnectTo = availableDoors.Peek(); // choose next door to spawn at

            // raycast to see if door is too close to other room
            // if not good: remove door
            while (IsPlacementValid(doorToConnectTo.position, Quaternion.identity, new Vector3(5, 5, 5)) == false)
            {
                availableDoors.Dequeue();
                doorToConnectTo = availableDoors.Peek(); // choose next door to spawn at
            }

            GameObject spawningRoom = roomPrefabs[Random.Range(0, roomPrefabs.Length)]; // select random room

            GameObject dummy = Instantiate(spawningRoom);

            RoomInfo spawningRoomInfo = spawningRoom.GetComponent<RoomInfo>();
            if (spawningRoomInfo == null || spawningRoomInfo.doorPoints.Length == 0)
                continue;

            // select a door
            Transform selectedSpawingDoor = spawningRoomInfo.doorPoints[Random.Range(0, spawningRoomInfo.doorPoints.Length)];
            Debug.Log("connect selected door: " + selectedSpawingDoor.parent.name + " to " + doorToConnectTo.parent.name);
            
            /*
             * Rotate room to be same rotation as old door
             * Put new door at same position as old door
             */
            Vector3 horizontalCurrentForward = new Vector3(doorToConnectTo.forward.x, 0, doorToConnectTo.forward.z).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(horizontalCurrentForward);
            Quaternion newRoomRotation = targetRotation * Quaternion.Inverse(selectedSpawingDoor.rotation);
            Debug.Log("horizontalCurrentForward: " + horizontalCurrentForward + "  target: " + targetRotation.eulerAngles + "  new rot: " + newRoomRotation.eulerAngles);

            Vector3 doorOffset = selectedSpawingDoor.position - spawningRoom.transform.position;
            Vector3 newRoomPosition = doorToConnectTo.position - newRoomRotation * doorOffset; // get current door's real world position
            Debug.Log("curr door: " + doorToConnectTo.position + "  rot: " + newRoomRotation.eulerAngles + "  offset: " + doorOffset);
            Debug.Log("new room position: " + newRoomPosition);

            yield return new WaitForSeconds(0.3f);
            dummy.transform.rotation = newRoomRotation;
            dummy.transform.position = newRoomPosition;
            yield return new WaitForSeconds(0.75f);

            BoxCollider box = spawningRoom.GetComponent<BoxCollider>();
            Vector3 boxCenter = box.transform.TransformPoint(box.center);
            
            Vector3 boxPosition = newRoomPosition + boxCenter;
            Vector3 rotatedCenter = RotatePointAroundPivot(boxPosition, newRoomPosition, newRoomRotation.eulerAngles); // rotated room center
            
            Destroy(dummy);
            
            yield return new WaitForSeconds(0.3f);
            if (IsPlacementValid(rotatedCenter, newRoomRotation, spawningRoom.GetComponent<BoxCollider>().size) == false)
            {
                Debug.Log("exit loop");
                yield return new WaitForSeconds(1f);
                continue;
            }

            yield return new WaitForSeconds(0.4f);

            roomCount++;
            GameObject newRoom = Instantiate(spawningRoom, newRoomPosition, newRoomRotation);
            newRoom.transform.SetParent(transform);
            placedRooms.Add(newRoom);

            yield return new WaitForSeconds(0.4f);

            // add room's doors to list
             if (spawningRoomInfo != null)
            {
                RoomInfo newRoomInfo = newRoom.GetComponent<RoomInfo>();
                Vector3 selectedDoorPos = newRoom.transform.position + selectedSpawingDoor.position; // get selectedDoor's world position
                foreach (Transform door in newRoomInfo.doorPoints)
                {
                    if (door.position != selectedDoorPos) // don't add attached door
                        availableDoors.Enqueue(door);
                }
            }

            availableDoors.Dequeue(); // door was successful so remove

            yield return new WaitForSeconds(0.5f);
        }

        // either no more available doors or maxRooms was achieved
        if (placedRooms.Count <= minRooms)
        {
            Debug.LogWarning("No more available doors and too few rooms placed. Retrying...");
            retryNum++;
            yield return null;
            ClearAllRooms();
            BuildHouse();
        }
        else // success
        {
            RemoveOverlappingDoors();
            if (surface)
                surface.BuildNavMesh();

            roomsFinished.Invoke();
        }
    }


    // gizmo helpers
    Vector3 roomPlacementTransform;
    Vector3 roomSize;
    Quaternion orientation = Quaternion.identity;

    /// <summary>
    /// Takes "roomPrefab" and checks if its collider overlaps any other Room colliders
    /// </summary>
    bool IsPlacementValid(Vector3 position, Quaternion rotation, Vector3 size)
    {
        //BoxCollider roomCollider = roomPrefab.GetComponent<BoxCollider>();

        // place overlap box using room position
        roomPlacementTransform = position;
        orientation = rotation;
        roomSize = size;

        // Check for overlap
        Collider[] hitColliders = Physics.OverlapBox(position, roomSize / 2, rotation);
        foreach (Collider hit in hitColliders)
        {
            GameObject hitObject = hit.gameObject;
            if (hitObject.CompareTag("Room"))
            {
                Debug.Log("BAD placement collided with: " + hitObject.name);
                return false;
            }
        }
        Debug.Log("GOOD placement");
        return true;
    }

    /// <summary>
    /// Get position of "point" when rotated to "angles" around "pivot"
    /// </summary>
    private Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        return Quaternion.Euler(angles) * (point - pivot) + pivot;
    }

    private void OnDrawGizmos()
    {
        DrawBox(roomPlacementTransform, orientation, roomSize, Color.red);
    }

    /// <summary>
    /// Checks rooms if there are any overlapping doors and chooses one to remove
    /// </summary>
    void RemoveOverlappingDoors()
    {
        GameObject[] doorList = GameObject.FindGameObjectsWithTag("Door");
        List<GameObject> removedDoors = new List<GameObject>();
        foreach (GameObject door in doorList)
        {
            if (removedDoors.Contains(door))
                continue;

            Collider doorCollider = door.GetComponent<Collider>();
            if (doorCollider == null)
                continue;

            Vector3 boxCenter = doorCollider.bounds.center;
            Vector3 boxSize = doorCollider.bounds.size;

            // Overlap box for door
            Collider[] hits = Physics.OverlapBox(boxCenter, boxSize, door.transform.rotation);

            foreach (Collider hit in hits)
            {
                GameObject hitDoor = hit.gameObject;
                if (hitDoor == door)
                    continue; // Check for self
                else if (removedDoors.Contains(hitDoor))
                    continue;
                else if (hitDoor.CompareTag("Door"))
                {
                    Destroy(door);
                    removedDoors.Add(hitDoor);
                    break;
                }
            }
        }
    }
    
    // https://gist.github.com/unitycoder/58f4b5d80f423d29e35c814a9556f9d9
    /// <summary>
    /// Makes a rotated gizmo cube
    /// </summary>
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

    void ClearAllRooms()
    {
        foreach (GameObject room in placedRooms)
        {
            Destroy(room);
        }
        placedRooms.Clear();
        availableDoors.Clear();
        roomCount = 0;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = UnityEngine.Color.green;
        Gizmos.DrawSphere(levelSpawnPosition, 0.5f);
    }
}
