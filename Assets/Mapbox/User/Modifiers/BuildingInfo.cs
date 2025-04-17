using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingInfo : MonoBehaviour
{
    public int difficulty = 1;
    public int floors = 1;
    public float meshX, meshY;

    private void Start()
    {
        MeshCollider parentMesh = GetComponentInParent<MeshCollider>();

        // get size of building and assign difficulty and floors
        Vector3 meshSize = parentMesh.bounds.size;
        meshX = meshSize.x; meshY = meshSize.y;
        difficulty = (int)(meshSize.x + meshSize.y);
        if (meshSize.y < 3)
            floors = 1;
        else if (meshSize.y < 6)
            floors = 2;
        else
            floors = 3;

        transform.parent.tag = "Wall";
    }
}
