using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    public NavMeshSurface surface;
    public List<GameObject> roomPrefabs;
    public int maxRooms = 10;
    private List<Transform> availableDoors = new List<Transform>();
    private List<GameObject> placedRooms = new List<GameObject>();
    private int roomCount = 0;
    void Start()
    {
        GameObject startRoom = Instantiate(roomPrefabs[Random.Range(0,roomPrefabs.Count - 1)], Vector3.zero, Quaternion.identity);
        roomCount++;
        
        RoomInfo startRoomScript = startRoom.GetComponent<RoomInfo>();
        if (startRoomScript != null)
        {
            availableDoors.AddRange(startRoomScript.doorPoints);
        }

        StartCoroutine(GenerateRooms());
        if(surface){
            surface.BuildNavMesh();
        }
    }

    IEnumerator GenerateRooms()
    {
        while (availableDoors.Count > 0 && roomCount < maxRooms)
        {
            // Select possible door
            Transform currentDoor = availableDoors[Random.Range(0,availableDoors.Count - 1)];
            availableDoors.RemoveAt(0);
            
            GameObject spawningRoom = roomPrefabs[Random.Range(0, roomPrefabs.Count)];
            RoomInfo newRoomScript = spawningRoom.GetComponent<RoomInfo>();
            if (newRoomScript == null || newRoomScript.doorPoints.Length == 0)
            {
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
                continue;
            }

            // Spawn room
            GameObject newRoom = Instantiate(spawningRoom, newRoomPosition, newRoomRotation);
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
            yield return new WaitForSeconds(.2f);
        }
        DoorSelect();
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
            Collider[] hits = Physics.OverlapBox(boxCenter, boxSize / 2f, door.transform.rotation);

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
        
        Debug.Log(hitColliders);
        return (hitColliders.Length == 0);
    }
}
