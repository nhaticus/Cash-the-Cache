using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingInfo : MonoBehaviour
{
    public int difficulty = 1; // range: 1-5
    public int floors = 1; // range: 1-3
    public float meshX, meshY;

    private void Start()
    {
        MeshCollider parentMesh = GetComponentInParent<MeshCollider>();

        // get size of building and assign difficulty and floors
        Vector3 meshSize = parentMesh.bounds.size;
        meshX = meshSize.x; meshY = meshSize.y;
        difficulty = (int) Mathf.Floor((meshSize.x + meshSize.y) / 10 + Random.Range(0.3f, 1));
        if (meshSize.y < 3)
            floors = 1;
        else if (meshSize.y < 6)
            floors = 2;
        else
            floors = 3;

        transform.parent.tag = "Wall";
    }
}
