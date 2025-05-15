using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    [Header("Setup (optional)")]
    public GameObject startRoomPrefab;
    Vector3 levelSpawnPosition;
    public int maxRooms = 10;
    public NavMeshSurface surface;

    [Header("House Rooms")]
    public List<GameObject> roomPrefabs;
    [Header("Other Rooms")] // not yet anything but for future reference
    private List<Transform> availableDoors = new List<Transform>();
    private List<GameObject> placedRooms = new List<GameObject>();
    private int roomCount = 0;
    private GameObject startRoom;
    void Start()
    {
        maxRooms = PlayerPrefs.GetInt("Difficulty", 4) * 3; // dumb way for now
        BuildHouse();
    }

    public void BuildHouse(){
        levelSpawnPosition = transform.position;
        if(startRoomPrefab){
            startRoom = Instantiate(startRoomPrefab, levelSpawnPosition, Quaternion.identity);
            startRoom.transform.SetParent(this.transform);
        }
        else { 
            startRoom = Instantiate(roomPrefabs[Random.Range(0,roomPrefabs.Count - 1)], levelSpawnPosition, Quaternion.identity);
            startRoom.transform.SetParent(this.transform);
        }
        roomCount++;
        
        RoomInfo startRoomScript = startRoom.GetComponent<RoomInfo>();
        if (startRoomScript != null)
        {
            availableDoors.AddRange(startRoomScript.doorPoints);
        }

        StartCoroutine(GenerateRooms());
    }

    IEnumerator GenerateRooms()
    {
        while (availableDoors.Count > 0 && roomCount < maxRooms)
        {
            Debug.Log("roomCount: " + roomCount + " < " + maxRooms);
            // Select possible door
            int randomDoor = Random.Range(0,availableDoors.Count - 1);
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
        DoorSelect();
        if(surface){
            surface.BuildNavMesh();
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

    // Referenced from ChatGPT
    void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(levelSpawnPosition, 0.5f);
    }
}
