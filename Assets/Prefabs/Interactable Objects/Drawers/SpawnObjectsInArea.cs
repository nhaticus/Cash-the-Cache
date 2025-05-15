using UnityEngine;

public class SpawnObjectsInArea : MonoBehaviour
{
    [SerializeField] BoxCollider area;

    [SerializeField] GameObject[] prefabs;
    [SerializeField] int spawnMin = 0, spawnMax = 0;
    int amtOfObjects = 0;

    float xRange = 1, yRange = 1, zRange = 1;

    void Start()
    {
        area = GetComponent<BoxCollider>();
        amtOfObjects = Random.Range(spawnMin, spawnMax);
        xRange = area.size.x; yRange = area.size.y; zRange = area.size.z;
        for (int i = 0; i < amtOfObjects; i++)
        {
            var px = Random.Range(-xRange, xRange);
            var py = Random.Range(-yRange, yRange);
            var pz = Random.Range(-zRange, zRange);
            Vector3 pos = new Vector3(px, py, pz);
            GameObject child = Instantiate(prefabs[Random.Range(0, prefabs.Length - 1)], transform.position + pos, Quaternion.identity);
            child.transform.parent = this.transform;
        }
        
    }

    void OnDrawGizmos()
    {
        // Draw a cube at the transform position
        Gizmos.color = new Color(0, 1, 0, 0.2f);
        Gizmos.DrawCube(transform.position, area.size);
    }

}
