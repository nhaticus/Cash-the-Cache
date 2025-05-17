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
    public float spawnerRadius = 20.0f; // Radius to spawn NPCs arpund

    void Start()
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
        int maxSpawnAttempts = NPC.spawnCount * spawnAttemptsPerNPC; // Limit the number of attempts to avoid infinite loops (10 attempts per count)
        while (spawnedCount < NPC.spawnCount && spawnAttempts < maxSpawnAttempts)
        {
            spawnAttempts++;
            Debug.Log("Spawning NPC #: " + spawnedCount);
            Vector3 randomPoint = Random.insideUnitSphere * spawnerRadius;
            randomPoint.y += 5;
            if (Physics.Raycast(randomPoint, Vector3.down, out RaycastHit hit, 10f, NPC.spawnLayer))
            {
                Instantiate(NPC.NPCPrefab, hit.point, Quaternion.identity);
                spawnedCount++;
            }
        }

        if (spawnedCount < NPC.spawnCount)
        {
            Debug.Log("only spawwned " + spawnedCount + " NPCs out of " + NPC.spawnCount);
        }
    }

    void SpawnPolice()
    {
        foreach (NPCSpawnData Police in PoliceList)
        {
            SpawnNPC(Police);
        }
    }

    void OnEnable()
    {
        GameManager.Instance.SpawnPolice += SpawnPolice;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnerRadius);
    }
}
