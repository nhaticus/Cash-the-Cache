using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PoliceBehavior : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform player;

    /*  Navmesh Agent Settings   */
    [Header("Navmesh Agent Settings")]
    public float speed = 3.5f;

    /*  Layers for detection    */
    [Header("Layers for Detection")]
    public LayerMask playerLayer;
    public LayerMask groundLayer;

    /*  Random walking  */
    [Header("Patrolling Settings")]
    [SerializeField] private Vector3 walkPoint;
    private bool walkPointExist;
    public float walkPointRange;

    /*  Detection  */
    [Header("Detection Settings")]
    public float stunDuration;
    public float sightDistance;
    public float reachDistance;
    private bool withinSight, withinReach;

    /*  Timers  */
    private bool alreadyChasing = false;
    public float chaseDuration = 3.0f;   // how long police keep chasing once triggered
    private float chaseTimer = 0.0f;     // counts down after chase starts

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        player = GameObject.Find("Player").transform;
        if (stunDuration == 0f)
            stunDuration = 1.0f;
    }

    void Update()
    {
        if (Physics.Raycast(transform.position, -transform.up, 2f, groundLayer))
            DetectPlayer();
        else
            agent.SetDestination(player.position);
    }

    private void DetectPlayer()
    {
        // 1) Determine if player is in sight cone
        withinSight = false;
        for (int i = -45; i <= 45; i += 5)
        {
            Vector3 dir = Quaternion.Euler(0, i, 0) * transform.forward;
            Debug.DrawRay(transform.position, dir * sightDistance, Color.red);
            if (Physics.Raycast(transform.position, dir, out RaycastHit hit, sightDistance))
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("playerLayer"))
                {
                    withinSight = true;
                    break;
                }
            }
        }

        // 2) Check if within attack range
        withinReach = Physics.CheckSphere(transform.position, reachDistance, playerLayer);

        // 3) Trigger the chase timer as soon as any NPC’s bar is full
        if (chaseTimer <= 0f)
        {
            foreach (var npc in FindObjectsOfType<NPCsBehavior>())
            {
                if (npc.GetDetectionRatio() >= 1f)
                {
                    chaseTimer = chaseDuration;
                    alreadyChasing = true;
                    Debug.Log($"[PoliceBehavior] Chase timer started at {Time.time:F2}s because '{npc.name}' notice bar full");
                    break;
                }
            }
        }

        // 4) If out of sight but still in chase window, count it down
        if (!withinSight && chaseTimer > 0f)
            chaseTimer -= Time.deltaTime;
        else if (!withinSight && chaseTimer <= 0f)
            alreadyChasing = false;

        // 5) Decide action
        if ((alreadyChasing || chaseTimer > 0f) && !withinReach)
            ChasePlayer();
        else if (withinSight && withinReach)
        {
            chaseTimer = 0f;
            AttackPlayer();
        }
        else
            PathingDefault();
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
        transform.LookAt(player);
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(player);
        GameManager.Instance.SetGameState(GameManager.GameState.Over);
    }

    private void PathingDefault()
    {
        if (!walkPointExist) FindWalkPoint();
        if (walkPointExist)
            agent.SetDestination(walkPoint);

        if ((transform.position - walkPoint).magnitude < 1f)
        {
            walkPointExist = false;
            StartCoroutine(WaitBeforeMoving());
        }
    }

    private IEnumerator WaitBeforeMoving()
    {
        agent.isStopped = true;
        yield return new WaitForSeconds(1f);
        agent.isStopped = false;
    }

    private void FindWalkPoint()
    {
        float rz = Random.Range(-walkPointRange, walkPointRange);
        float rx = Random.Range(-walkPointRange, walkPointRange);
        walkPoint = new Vector3(transform.position.x + rx,
                                transform.position.y,
                                transform.position.z + rz);
        if (Physics.Raycast(walkPoint, -transform.up, 2f, groundLayer))
            walkPointExist = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, reachDistance);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightDistance);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bat"))
            StartCoroutine(Stun());
    }

    private IEnumerator Stun()
    {
        agent.isStopped = true;
        yield return new WaitForSeconds(stunDuration);
        agent.isStopped = false;
    }
}
