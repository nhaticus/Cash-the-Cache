using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingInfo : MonoBehaviour
{
    public int difficulty = 1;
    public int floors = 1;

    private void Start()
    {
        MeshCollider parentMesh = GetComponentInParent<MeshCollider>();

        Vector3 meshSize = parentMesh.bounds.size;
        difficulty = (int)(meshSize.x + meshSize.y);

    }
}
