using System.Collections;
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
    public NPCSpawnData[] NPCList;  // NPCs to spawn
    public NPCSpawnData[] PoliceList; // Police to spawn

    [Header("Spawn Settings")]
    [SerializeField] int amountOfNPCs = 2;
    public int spawnAttemptsPerNPC = 10;
    public float spawnerRadius = 20.0f; // Radius to spawn NPCs around

    [SerializeField] GameObject policeSpawnPoint;

    private void Start()
    {
        SetDifficulty();
    }

    void SetDifficulty()
    {
        amountOfNPCs += (int) Mathf.Floor(0.14f * DataSystem.Data.gameState.currentReplay + PlayerPrefs.GetInt("Difficulty"));
        amountOfNPCs = Mathf.Min(amountOfNPCs, 8);
    }


    public void NPCSpawn()
    {
        GameManager.Instance.SpawnPolice += SpawnPolice;

        for(int i = 0; i < amountOfNPCs; i++)
        {
            NPCSpawnData NPC = NPCList[Random.Range(0, NPCList.Length)];
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

            // raycast to the ground and spawn NPC
            if (Physics.Raycast(randomPoint, Vector3.down, out RaycastHit hit, 10f, NPC.spawnLayer))
            {
                Instantiate(NPC.NPCPrefab, hit.point, Quaternion.identity);
                spawnedCount++;
            }
        }
    }

    [SerializeField] float policeSpawnDelay = 0.3f;
    void SpawnPolice()
    {
        if(PlayerManager.Instance && PlayerManager.Instance.isPlayerActive)
            StartCoroutine(DelaySpawnPolice(policeSpawnDelay));
    }

    IEnumerator DelaySpawnPolice(float delayTime)
    {
        foreach (NPCSpawnData Police in PoliceList)
        {
            for (int i = 0; i < Police.spawnCount; i++)
            {
                Instantiate(Police.NPCPrefab, policeSpawnPoint.transform.position, Quaternion.identity);
                yield return new WaitForSeconds(delayTime);
            }
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
