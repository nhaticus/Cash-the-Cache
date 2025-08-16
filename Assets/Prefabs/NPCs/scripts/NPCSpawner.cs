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
    [SerializeField] int amountOfNPCs = 2;
    public int spawnAttemptsPerNPC = 10; // Number of attempts to spawn each NPC
    public float spawnerRadius = 20.0f; // Radius to spawn NPCs around

    private void Start()
    {
        SetDifficulty();
    }

    void SetDifficulty()
    {
        Debug.Log("Spawn more: " + (int)Mathf.Floor(0.15f * DataSystem.Data.gameState.currentReplay + PlayerPrefs.GetInt("Difficulty")));
        amountOfNPCs += (int) Mathf.Floor(0.15f * DataSystem.Data.gameState.currentReplay + PlayerPrefs.GetInt("Difficulty"));
        Debug.Log("total spawn: " + amountOfNPCs);
    }


    public void NPCSpawn()
    {
        GameManager.Instance.SpawnPolice += SpawnPolice;

        for(int i = 0; i < amountOfNPCs; i++)
        {
            Debug.Log("spawn: " + i);
            NPCSpawnData NPC = NPCList[Random.Range(0, NPCList.Length)];
            SpawnNPC(NPC);
        }
    }

    void SpawnNPC(NPCSpawnData NPC)
    {
        Debug.Log("called spawn");
        int spawnedCount = 0;
        int spawnAttempts = 0;
        int maxSpawnAttempts = NPC.spawnCount * spawnAttemptsPerNPC; // Limit the number of attempts to avoid infinite loops 
        while (spawnedCount < NPC.spawnCount && spawnAttempts < maxSpawnAttempts)
        {
            spawnAttempts++;

            Vector3 randomPoint = Random.insideUnitSphere * spawnerRadius + transform.position;
            randomPoint.y = 1;

            // raycast to the ground and spawn NPC
            if (Physics.Raycast(randomPoint, Vector3.down, out RaycastHit hit, 10f, NPC.spawnLayer))
            {
                Instantiate(NPC.NPCPrefab, hit.point, Quaternion.identity);
                spawnedCount++;
            }
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
