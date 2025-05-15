using System.Drawing;
using UnityEngine;

public class SpawnObjectsInArea : MonoBehaviour
{
    [SerializeField] GameObject[] prefabs;
    [SerializeField] int amtOfObjects = 0;

    [SerializeField] float xRange = 1, yRange = 1, zRange = 1;
    Vector3 spawnArea;

    void Start()
    {
        spawnArea = new Vector3(xRange, yRange, zRange);
        for (int i = 0; i < amtOfObjects; i++)
        {
            var px = Random.Range(-xRange, xRange);
            var py = Random.Range(-yRange, yRange);
            var pz = Random.Range(-zRange, zRange);
            Vector3 pos = new Vector3(px, py, pz);
            Instantiate(prefabs[Random.Range(0, prefabs.Length - 1)], transform.position + pos, Quaternion.identity);
        }
        
    }

    void OnDrawGizmos()
    {
        // Draw a yellow cube at the transform position
        Gizmos.color = UnityEngine.Color.green;
        Gizmos.DrawWireCube(transform.position, spawnArea);
    }

}
