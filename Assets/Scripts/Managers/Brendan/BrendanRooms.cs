using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Events;

public class BrendanRooms : MonoBehaviour
{
    [Header("Setup (optional)")]
    public GameObject startRoomPrefab;
    Vector3 levelSpawnPosition;
    Quaternion levelSpawnRotation;
    public int maxRooms = 10;
    public int minRooms = 5;
    public int maxRetries = 30;
    public NavMeshSurface surface;

    [Header("House Rooms")]
    public List<GameObject> roomPrefabs;
    [Header("Other Rooms")] // not yet anything but for future reference

    [Header("AI Rooms")]
    public List<GameObject> aiRoomPrefabs;
    private List<Transform> availableDoors = new List<Transform>();
    private List<GameObject> placedRooms = new List<GameObject>();
    private int roomCount = 0;
    private int retryNum = 0;
    private GameObject startRoom;

    // send event when All Rooms Generated
    public UnityAction roomsFinished;

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
        if (startRoomPrefab == null)
            startRoomPrefab = roomPrefabs[Random.Range(0, roomPrefabs.Count - 1)];

        startRoom = Instantiate(startRoomPrefab, levelSpawnPosition, levelSpawnRotation);
        startRoom.transform.SetParent(transform);
        placedRooms.Add(startRoom);
        roomCount++;
        RoomInfo startRoomScript = startRoom.GetComponent<RoomInfo>();
        if (startRoomScript != null)
            availableDoors.AddRange(startRoomScript.doorPoints);
    }

    /// <summary>
    /// IEnumerator that determines which rooms to spawn and creates them all.<br />
    /// IEnumerator because it needs to be able to run without freezing the game to generate rooms.
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
            // Select possible door from list
            int randomDoor = Random.Range(0, availableDoors.Count - 1);

            Transform currentDoor = availableDoors[randomDoor]; // choose random door to spawn at
            availableDoors.RemoveAt(randomDoor);

            GameObject spawningRoom = roomPrefabs[Random.Range(0, roomPrefabs.Count)]; // select random room
            RoomInfo newRoomScript = spawningRoom.GetComponent<RoomInfo>();
            if (newRoomScript == null || newRoomScript.doorPoints.Length == 0)
            {
                continue;
            }

            // select a door
            Transform selectedDoor = newRoomScript.doorPoints[Random.Range(0, newRoomScript.doorPoints.Length)];

            // Align opposing directions
            Vector3 horizontalCurrentForward = new Vector3(currentDoor.forward.x, 0, currentDoor.forward.z).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(-horizontalCurrentForward);
            Quaternion newRoomRotation = targetRotation * Quaternion.Inverse(selectedDoor.rotation);

            // Use world offset between prefab origin and door
            Vector3 doorOffset = selectedDoor.position - spawningRoom.transform.position;
            Vector3 newRoomPosition = currentDoor.position - newRoomRotation * doorOffset;

            // Check for room overlap
            if (!IsPlacementValid(spawningRoom, newRoomPosition, newRoomRotation))
            {
                Debug.Log("overlap found");
                continue;
            }

            // Spawn room
            GameObject newRoom = Instantiate(spawningRoom, newRoomPosition, newRoomRotation);
            newRoom.transform.SetParent(transform);

            roomCount++;

            placedRooms.Add(newRoom);

            RoomInfo newRoomInstanceScript = newRoom.GetComponent<RoomInfo>();
            // add room's doors to list
            if (newRoomInstanceScript != null)
            {
                foreach (Transform door in newRoomInstanceScript.doorPoints)
                {
                    if (door != selectedDoor)
                        availableDoors.Add(door);
                }
            }

            yield return null;
        }

        // either there are no more available doors or maxRooms was achieved
        if (placedRooms.Count <= minRooms)
        {
            Debug.LogWarning("Too few rooms placed. Retrying...");
            Debug.Log(placedRooms.Count);
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
            Debug.Log("finished making rooms");
            roomsFinished.Invoke();
        }
    }

    /// <summary>
    /// Checks rooms if there are any overlapping doors and chooses one to remove
    /// </summary>
    void RemoveOverlappingDoors()
    {
        // list of doors from placedRooms
        /*
        List<GameObject> doorList = new List<GameObject>();
        for(int i = 0; i < placedRooms.Count; i++)
        {
            Transform[] doors = placedRooms[i].GetComponent<RoomInfo>().doorPoints;
            foreach(Transform door in doors)
            {
                doorList.Append(door.gameObject);
            }
        }
        */
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
                if (removedDoors.Contains(hitDoor))
                {
                    continue;
                }
                else if (hitDoor.CompareTag("Door"))
                {
                    Debug.Log("remove door");
                    //doorList.Remove(door);
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
            Debug.LogWarning("No BoxCollider found on the room prefab.");
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
                return false;
            }
        }
        return true;
    }

    void ClearAllNonStartRooms()
    {
        Debug.Log("remove all rooms");
        for(int i = placedRooms.Count - 1; i >= 0; i--)
        {
            GameObject room = placedRooms[i];
            if (room == startRoom)
                continue;

            placedRooms.Remove(room);
            foreach (Transform door in room.GetComponent<RoomInfo>().doorPoints)
            {
                availableDoors.Remove(door);
            }
            roomCount--;
            Destroy(room);
        }
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
