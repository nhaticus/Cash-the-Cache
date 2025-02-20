/*  This script is attached to NPC prefab
    * FUNCTIONALITY: NPC will wander around until detecting the player, after which it will run away to a designated exit point
    * HOW TO USE: drag the prefab into and set the exit point attached to the parent object to any entrance/exit point. Make sure to set the ground layer of the map to "Ground" inorder for the NPC to traverse it
    */
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NPCsBehavior : MonoBehaviour
{
    private NavMeshAgent agent;

    private Transform player;

    /*  Navmesh Agent Settings   */
    [Header("Navmesh Agent Settings")]
    public float speed = 3.5f; // default speed 3.5f

    /*  Layers for detection    */
    [Header("Layers for Detection")]
    public LayerMask playerLayer;
    public LayerMask groundLayer;

    /*  Random walking  */
    [Header("Patrolling Settings")]
    [SerializeField] private Vector3 walkPoint;
    private bool walkPointExist;
    public float walkPointRange;
    public Transform exit;

    /*  Detection  */
    [Header("Detection Settings")]
    public float sightDistance;
    private bool withinSight;
    private bool detectedPlayer = false;

    /*  Timers  */
    public float sightCountdown = 2.0f; // Time for how long the player needs to stay in line-of-sight before the enemy starts chasing
    private float sightTimer = 0.0f;
    public float chaseDuration = 3.0f; // Time for how long the enemy will chase the player before giving up

    private void Awake()
    {
        /*  Setting up variables    */
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        player = GameObject.Find("Player").transform;
    }

    void Update()
    {
        if (detectedPlayer)
        {
            Runaway();
        }
        else
        {
            DetectPlayer();
        }
    }

    private void DetectPlayer()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, sightDistance, playerLayer);
        if (hitColliders.Length == 0)
        {
            withinSight = false;
        }
        else
        {
            foreach (var hitCollider in hitColliders)
            {
                Vector3 directionToPlayer = (hitCollider.transform.position - transform.position).normalized;
                float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

                if (angleToPlayer <= 45)
                {
                    withinSight = true;
                    break;
                }
            }
        }

        // Debug.Log("Within Sight: " + withinSight + " Within Reach: " + withinReach);

        if (withinSight)
        {
            sightTimer += Time.deltaTime;
        }
        else
        {
            sightTimer = 0.0f;
        }

        if (sightTimer >= sightCountdown)
        {
            detectedPlayer = true;
        }
        else
        {
            // Idle
            PathingDefault();
        }

    }


    private void Runaway()
    {
        Vector3 distanceToExit = transform.position - exit.position;

        if (distanceToExit.magnitude < 1.0f)
        {
            Destroy(transform.parent.gameObject);
            Debug.Log("NPC has escaped!");
        }
        agent.SetDestination(exit.transform.position);

        Debug.Log("Player is in sight! RUN!");
        // trigger police here...
    }

    private void PathingDefault()
    {
        if (!walkPointExist) FindWalkPoint();

        else if (walkPointExist)
        {
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 2.0f)
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
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, groundLayer))
        {
            NavMeshPath path = new NavMeshPath();
            if (agent.CalculatePath(walkPoint, path) && path.status == NavMeshPathStatus.PathComplete)
            {
                walkPointExist = true;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightDistance);
    }

}
