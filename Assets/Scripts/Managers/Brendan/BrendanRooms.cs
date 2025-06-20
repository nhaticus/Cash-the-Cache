using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Events;

public class BrendanRooms : MonoBehaviour
{
    [Header("Setup (optional)")]
    public GameObject[] startRooms;
    public int maxRooms = 10;
    public int minRooms = 5;
    public int maxRetries = 30;
    public NavMeshSurface surface;

    Vector3 levelSpawnPosition;
    Quaternion levelSpawnRotation;

    [Header("House Rooms")]
    public GameObject[] roomPrefabs; // list of all rooms that can be spawned

    [Header("AI Rooms")]
    public List<GameObject> aiRoomPrefabs;

    Queue<Transform> availableDoors = new Queue<Transform>();
    List<GameObject> placedRooms = new List<GameObject>();
    int roomCount = 0;
    int retryNum = 0;

    // send event when All Rooms Generated
    public UnityEvent roomsFinished;

    void Start()
    {
        BuildHouse();
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
        // get starting room from either startRooms or roomPrefabs if startRooms is empty
        GameObject startRoomPrefab;
        if (startRooms.Length > 0)
            startRoomPrefab = startRooms[Random.Range(0, startRooms.Length - 1)];
        else
            startRoomPrefab = roomPrefabs[Random.Range(0, roomPrefabs.Length - 1)];

        GameObject startRoom = Instantiate(startRoomPrefab, levelSpawnPosition, levelSpawnRotation);
        startRoom.transform.SetParent(transform);
        placedRooms.Add(startRoom);
        roomCount++;

        RoomInfo startRoomScript = startRoom.GetComponent<RoomInfo>();
        // push all doors to door stack
        if (startRoomScript != null)
        {
            foreach (Transform door in startRoomScript.doorPoints)
            {
                availableDoors.Enqueue(door);
            }
        }
    }

    /// <summary>
    /// IEnumerator that determines which rooms to spawn and creates them all.<br />
    /// IEnumerator because it needs to run without freezing the game to generate rooms.
    /// </summary>
    /// <returns></returns>
    IEnumerator GenerateRooms()
    {
        if (retryNum > maxRetries)
        {
            minRooms--;
            retryNum = 0;
        }

        while (availableDoors.Count > 0 && roomCount < maxRooms)
        {
            Transform currentDoor = availableDoors.Dequeue(); // choose door at top of stack to spawn at

            GameObject spawningRoom = roomPrefabs[Random.Range(0, roomPrefabs.Length - 1)]; // select random room
            RoomInfo newRoomInfo = spawningRoom.GetComponent<RoomInfo>();
            if (newRoomInfo == null || newRoomInfo.doorPoints.Length == 0)
            {
                Debug.Log("no room info");
                continue;
            }

            // select a door
            Transform selectedDoor = newRoomInfo.doorPoints[Random.Range(0, newRoomInfo.doorPoints.Length)];

            // Align opposing directions
            Vector3 horizontalCurrentForward = new Vector3(currentDoor.forward.x, 0, currentDoor.forward.z).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(-horizontalCurrentForward);
            Quaternion newRoomRotation = targetRotation * Quaternion.Inverse(selectedDoor.rotation);

            // Use world offset between prefab origin and door
            Vector3 doorOffset = selectedDoor.position - spawningRoom.transform.position;
            Vector3 newRoomPosition = currentDoor.position - newRoomRotation * doorOffset;

            // Check for room overlap
            if (IsPlacementValid(spawningRoom, newRoomPosition, newRoomRotation) == false)
            {
                Debug.Log("bad placement");
                continue;
            }
                
            // Spawn room
            GameObject newRoom = Instantiate(spawningRoom, newRoomPosition, newRoomRotation);
            newRoom.transform.SetParent(transform);

            roomCount++;

            placedRooms.Add(newRoom);

            // add room's door points to list unless it is the currently spawned door point
            if (newRoomInfo != null)
            {
                foreach (Transform door in newRoomInfo.doorPoints)
                {
                    if(door != selectedDoor)
                        availableDoors.Enqueue(door);
                }
            }
            Debug.Log("added: " + availableDoors.Count);

            yield return null;
        }

        // either there are no more available doors or maxRooms was achieved
        if (placedRooms.Count <= minRooms)
        {
            retryNum++;
            yield return null;
            ClearAllRooms();
            BuildHouse();
        }
        else // success
        {
            RemoveOverlappingDoors();
            if (surface != null)
            {
                surface.BuildNavMesh();
            }
            roomsFinished.Invoke();
        }
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
            {
                continue;
            }
            
            Collider doorCollider = door.GetComponent<Collider>();
            if (doorCollider == null)
            {
                continue;
            }

            Vector3 boxCenter = doorCollider.bounds.center;
            Vector3 boxSize = doorCollider.bounds.size;

            // Overlap box for door
            Collider[] hits = Physics.OverlapBox(boxCenter, boxSize, door.transform.rotation);

            foreach (Collider hit in hits)
            {
                GameObject hitDoor = hit.gameObject;
                if (hitDoor == door)
                {
                    continue; // Check for self
                }
                else if (removedDoors.Contains(hitDoor))
                {
                    continue;
                }
                else if (hitDoor.CompareTag("Door"))
                {
                    Destroy(door);
                    removedDoors.Add(hitDoor);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Takes a room and checks if its collider overlaps any other Room colliders
    /// </summary>
    /// <param name="roomPrefab"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <returns></returns>
    bool IsPlacementValid(GameObject roomPrefab, Vector3 position, Quaternion rotation)
    {
        BoxCollider roomCollider = roomPrefab.GetComponent<BoxCollider>();
        if (roomCollider == null)
        {
            //Debug.LogWarning("No BoxCollider found on the room prefab.");
            return true;
        }

        Vector3 worldCenter = position + rotation * roomCollider.center;
        Vector3 halfExtents = roomCollider.size * 0.5f;

        // Check for overlap
        Collider[] hitColliders = Physics.OverlapBox(worldCenter, halfExtents, rotation);
        foreach (Collider hit in hitColliders)
        {
            GameObject hitObject = hit.gameObject;
            if (hitObject.CompareTag("Room"))
            {
                //return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Remove all placed rooms
    /// </summary>
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

    /// <summary>
    /// Loops through each placed room, checking if it has an AI room.
    /// </summary>
    /// <returns></returns>
    bool HasAICheck()
    {
        foreach (GameObject room in placedRooms)
        {
            foreach (GameObject aiRoom in aiRoomPrefabs)
            {
                if (room.name.Contains(aiRoom.name)) // Match by prefab name
                {
                    return true;
                }
            }
        }
        return false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(levelSpawnPosition, 0.5f);
    }
}
