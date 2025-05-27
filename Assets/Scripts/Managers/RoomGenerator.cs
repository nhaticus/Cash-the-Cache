using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Events;

public class RoomGenerator : MonoBehaviour
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
    public bool isComplete = false;

    // send event when All Rooms Generated
    public UnityEvent roomsFinished;

    void Start()
    {
        BuildHouse();
    }

    public void BuildHouse()
    {
        isComplete = false;
        levelSpawnPosition = transform.position;
        levelSpawnRotation = transform.rotation;
        if (startRoomPrefab)
        {
            startRoom = Instantiate(startRoomPrefab, levelSpawnPosition, levelSpawnRotation);
            startRoom.transform.SetParent(this.transform);
        }
        else
        {
            startRoom = Instantiate(roomPrefabs[Random.Range(0, roomPrefabs.Count - 1)], levelSpawnPosition, levelSpawnRotation);
            startRoom.transform.SetParent(this.transform);
        }
        roomCount++;

        RoomInfo startRoomScript = startRoom.GetComponent<RoomInfo>();
        if (startRoomScript != null)
        {
            availableDoors.AddRange(startRoomScript.doorPoints);
        }
        placedRooms.Add(startRoom);
        StartCoroutine(GenerateRooms());
    }

    IEnumerator GenerateRooms()
    {
        if (retryNum > maxRetries)
        {
            minRooms--;
            retryNum = 0;
        }
        isComplete = false;
        while (availableDoors.Count > 0 && roomCount < maxRooms)
        {
            // Select possible door
            int randomDoor = 0;
            if (availableDoors.Count >= 1)
            {
                randomDoor = Random.Range(0, availableDoors.Count - 1);
            }
            Transform currentDoor = availableDoors[randomDoor];
            availableDoors.RemoveAt(randomDoor);

            GameObject spawningRoom = roomPrefabs[Random.Range(0, roomPrefabs.Count)];
            RoomInfo newRoomScript = spawningRoom.GetComponent<RoomInfo>();
            if (newRoomScript == null || newRoomScript.doorPoints.Length == 0)
            {
                Debug.Log("no doors");
                continue;
            }

            Transform selectedDoor = newRoomScript.doorPoints[Random.Range(0, newRoomScript.doorPoints.Length)];

            // Align opposing directions
            Vector3 horizontalCurrentForward = new Vector3(currentDoor.forward.x, 0, currentDoor.forward.z).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(-horizontalCurrentForward);
            Quaternion newRoomRotation = targetRotation * Quaternion.Inverse(selectedDoor.rotation);

            // Use world offset between prefab origin and door
            Vector3 doorOffset = selectedDoor.position - spawningRoom.transform.position;
            Vector3 newRoomPosition = currentDoor.position - newRoomRotation * doorOffset;

            // Check for overlap
            if (!IsPlacementValid(spawningRoom, newRoomPosition, newRoomRotation))
            {
                Debug.Log("overlap found");
                continue;
            }

            // Spawn room
            GameObject newRoom = Instantiate(spawningRoom, newRoomPosition, newRoomRotation);
            newRoom.transform.SetParent(this.transform);

            roomCount++;

            placedRooms.Add(newRoom);

            RoomInfo newRoomInstanceScript = newRoom.GetComponent<RoomInfo>();
            if (newRoomInstanceScript != null)
            {
                foreach (Transform door in newRoomInstanceScript.doorPoints)
                {
                    if (door != selectedDoor)
                    {
                        availableDoors.Add(door);
                    }
                }
            }
            yield return new WaitForSeconds(0f);
        }
        if (placedRooms.Count <= minRooms)
        {
            Debug.LogWarning("Too few rooms placed. Retrying...");
            Debug.Log(placedRooms.Count);
            retryNum++;
            yield return new WaitForSeconds(0f); // Optional small delay
            ClearLevel();
            BuildHouse();
            yield break;
        }
        else
        {
            DoorSelect();
            if (surface)
            {
                surface.BuildNavMesh();
            }
            GameObject npcGen = GameObject.Find("NPCGen");
            if (npcGen)
            {
                NPCSpawner npcGenScript = npcGen.GetComponent<NPCSpawner>();
                npcGenScript.NPCSpawn();
            }
            isComplete = true;
            roomsFinished.Invoke();
        }
    }

    void DoorSelect(){
        GameObject[] doorList = GameObject.FindGameObjectsWithTag("Door");
        HashSet<GameObject> removedDoors = new HashSet<GameObject>();
        foreach(GameObject door in doorList)
        {
            if(removedDoors.Contains(door)){
                continue;
            }

            Collider doorCollider = door.GetComponent<Collider>();
            if (doorCollider == null){
                continue;
            }

            Vector3 boxCenter = doorCollider.bounds.center;
            Vector3 boxSize = doorCollider.bounds.size;

            // Overlap box for door
            Collider[] hits = Physics.OverlapBox(boxCenter, boxSize, door.transform.rotation);

            foreach (Collider hit in hits)
            {
                GameObject hitDoor = hit.gameObject;
                if (hitDoor == door){
                    continue; // Check for self
                }
                if (!hitDoor.CompareTag("Door")){
                    continue;
                }
                if (removedDoors.Contains(hitDoor)){
                    continue;
                }
                Destroy(door);
                removedDoors.Add(hitDoor);
                break;
            }
        }
    }
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
        foreach(Collider hit in hitColliders){
            GameObject hitObject = hit.gameObject;
            if(hitObject.CompareTag("Room")){
                return(false);
            }
        }
        return (true);
    }

    void ClearLevel()
    {
        foreach (GameObject room in placedRooms)
        {
            Destroy(room);
        }
        placedRooms.Clear();
        availableDoors.Clear();
        roomCount = 0;
    }

    bool HasAICheck()
    {
        if (aiRoomPrefabs.Count > 0)
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
        return true;
    }

    // Referenced from ChatGPT
    void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(levelSpawnPosition, 0.5f);
    }

    

}
