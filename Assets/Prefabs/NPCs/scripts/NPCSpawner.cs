using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    [System.Serializable]
    public class NPCSpawnData
    {
        public GameObject NPCPrefab;
        public int spawnCount;
        public LayerMask spawnLayer;
    }

    [Header("NPC Spawn Data")]
    public NPCSpawnData[] NPCList;  // NPCs to spawn (assigned in the inspector)
    public NPCSpawnData[] PoliceList; // NPCs to spawn (assigned in the inspector)

    [Header("Spawn Settings")]
    public int spawnAttemptsPerNPC = 10; // Number of attempts to spawn each NPC
    public float spawnerRadius = 20.0f; // Radius to spawn NPCs around

    public void NPCSpawn()
    {
        GameManager.Instance.SpawnPolice += SpawnPolice;

        foreach (NPCSpawnData NPC in NPCList)
        {
            SpawnNPC(NPC);
        }
    }

    void SpawnNPC(NPCSpawnData NPC)
    {
        int spawnedCount = 0;
        int spawnAttempts = 0;
        int maxSpawnAttempts = NPC.spawnCount * spawnAttemptsPerNPC; // Limit the number of attempts to avoid infinite loops 
        while (spawnedCount < NPC.spawnCount && spawnAttempts < maxSpawnAttempts)
        {
            spawnAttempts++;

            Vector3 randomPoint = Random.insideUnitSphere * spawnerRadius + transform.position;
            randomPoint.y = 1;

            //Debug.DrawRay(randomPoint, Vector3.down * 10f, Color.red, 1f);
            // raycast to the ground and spawn NPC
            if (Physics.Raycast(randomPoint, Vector3.down, out RaycastHit hit, 10f, NPC.spawnLayer))
            {
                Instantiate(NPC.NPCPrefab, hit.point, Quaternion.identity);
                spawnedCount++;
            }
        }

        if (spawnedCount < NPC.spawnCount)
        {
            // Debug.Log("only spawned " + spawnedCount + " NPCs out of " + NPC.spawnCount);
        }
    }

    void SpawnPolice()
    {
        foreach (NPCSpawnData Police in PoliceList)
        {
            SpawnNPC(Police);
        }
    }

    /// <summary>
    /// Spawn Area drawn
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnerRadius);
    }
}
