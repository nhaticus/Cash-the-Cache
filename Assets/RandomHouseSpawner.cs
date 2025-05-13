using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomHouseSpawner : MonoBehaviour
{

    [Header("House Prefabs")]
    public GameObject[] housePrefabs; 

    [Header("Materials")]
    public Material[] possibleMaterials; 


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
        GameObject house = Instantiate(selectedPrefab, transform.position, Quaternion.identity);


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


    }
}
