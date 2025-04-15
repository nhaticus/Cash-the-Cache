using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    public List<GameObject> roomPrefabs;
    public int maxRooms = 10;
    private List<Transform> availableDoors = new List<Transform>();
    private int roomCount = 0;
    void Start()
    {
        GameObject startRoom = Instantiate(roomPrefabs[0], Vector3.zero, Quaternion.identity);
        roomCount++;
        
        RoomInfo startRoomScript = startRoom.GetComponent<RoomInfo>();
        if (startRoomScript != null)
        {
            availableDoors.AddRange(startRoomScript.doorPoints);
        }

        GenerateRooms();
    }

    void GenerateRooms()
    {
        while (availableDoors.Count > 0 && roomCount < maxRooms)
        {
            // Select possible door
            Transform currentDoor = availableDoors[0];
            availableDoors.RemoveAt(0);
            
            GameObject spawningRoom = roomPrefabs[Random.Range(0, roomPrefabs.Count)];
            RoomInfo newRoomScript = spawningRoom.GetComponent<RoomInfo>();
            if (newRoomScript == null || newRoomScript.doorPoints.Length == 0)
            {
                continue;
            }

            Transform selectedDoor = newRoomScript.doorPoints[Random.Range(0, newRoomScript.doorPoints.Length)];

            Vector3 horizontalCurrentForward = new Vector3(currentDoor.forward.x, 0, currentDoor.forward.z).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(-horizontalCurrentForward);
            Quaternion newRoomRotation = targetRotation * Quaternion.Inverse(selectedDoor.localRotation);

            Vector3 doorOffset = newRoomRotation * selectedDoor.localPosition;
            Vector3 newRoomPosition = currentDoor.position - doorOffset;

            if (!IsPlacementValid(spawningRoom, newRoomPosition, newRoomRotation))
            {
                continue;
            }
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
