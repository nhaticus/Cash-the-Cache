using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomHouseSpawner : MonoBehaviour
{

    [Header("House Prefabs")]
    public GameObject[] housePrefabs; 

    [Header("Materials")]
    public Material[] possibleMaterials;


    [Header("Path Spawning")]
    [SerializeField] GameObject pathTilePrefab;      // Your SM_Env_Path_01
    [SerializeField] int maxPathTiles = 10;          // Safety cap
    [SerializeField] float tileLength = 1f;          // Length of one path tile
    [SerializeField] string sidewalkTag = "SidewalkStopper"; // Tag for stop trigger

    // Start is called before the first frame update
    void Start()
    {
        if (housePrefabs.Length == 0)
        {
            Debug.LogError("No house prefabs assigned to the spawner.");
            return;
        }

        //Pick Random house prefab
        GameObject  selectedPrefab = housePrefabs[Random.Range(0, housePrefabs.Length)];

        //Spawn it at gameobject position
        GameObject house = Instantiate(selectedPrefab, transform.position, transform.rotation);


        //Get the MeshRenderer component from the house prefab
        MeshRenderer meshRenderer = house.GetComponentInChildren<MeshRenderer>();


        //Choose random material to make them look different
        if (meshRenderer != null && possibleMaterials.Length > 0)
        {
            Material[] newMats = meshRenderer.materials;

            if (newMats.Length > 0)
                newMats[0] = possibleMaterials[Random.Range(0, possibleMaterials.Length)];
            if (newMats.Length > 1)
                newMats[1] = possibleMaterials[Random.Range(0, possibleMaterials.Length)];

            meshRenderer.materials = newMats;
        }
        else
        {
            Debug.LogWarning("MeshRenderer missing or no materials provided!");
        }

        // Try to find the door anchor
        Transform doorPos = house.transform.Find("DoorPosition");
        if (doorPos != null)
        {
            SpawnPathFromDoor(doorPos);
        }
        else
        {
            Debug.LogWarning($"No DoorPosition found on {selectedPrefab.name}.");
        }
    }
    //void SpawnStretchedPath(Transform doorPos)
    //{
    //    Vector3 start = doorPos.position;
    //    Vector3 dir = doorPos.forward;

    //    float maxDist = 20f;
    //    RaycastHit hit;

    //    if (Physics.Raycast(start, dir, out hit, maxDist))
    //    {
    //        if (hit.collider.CompareTag(sidewalkTag))
    //        {
    //            float distance = hit.distance;

    //            // Spawn at the start (near the door)
    //            GameObject path = Instantiate(pathTilePrefab, start, Quaternion.LookRotation(dir));

    //            // Scale it forward (stretch along Z)
    //            Vector3 originalScale = path.transform.localScale;
    //            path.transform.localScale = new Vector3(originalScale.x, originalScale.y, distance);

    //            // Move the object so it stretches forward (from back/pivot)
    //            path.transform.position += dir * (distance / 2f);
    //        }
    //    }
    //    else
    //    {
    //        Debug.LogWarning("No sidewalk hit found from: " + doorPos.name);
    //    }
    //}
    void SpawnPathFromDoor(Transform doorPos)
    {
        Vector3 dir = doorPos.forward;
        Vector3 start = doorPos.position;

        for (int i = 0; i < maxPathTiles; i++)
        {
            Vector3 pos = start + dir * (i * tileLength);

            // Look ahead to where the *next* tile would go
            Vector3 checkPos = start + dir * ((i + 1) * tileLength);
            Collider[] hits = Physics.OverlapBox(checkPos, Vector3.one * 0.5f);
            bool shouldStop = false;

            foreach (Collider hit in hits)
            {
                if (hit.CompareTag(sidewalkTag))
                {
                    shouldStop = true;
                    break;
                }
            }

            // Spawn current tile
            Instantiate(pathTilePrefab, pos, Quaternion.LookRotation(dir));

            if (shouldStop)
                break;
        }
    }


}
