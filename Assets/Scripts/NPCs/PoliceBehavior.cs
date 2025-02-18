/*  This script is attached to Police prefab
    * FUNCTIONALITY: Police will be spawned outside the map through generator script and will path towards the player until they reach the house, 
    then they will start randomly walking around until they run into the player and chase them
    * HOW TO USE: Drag the prefab into any script and instantiate it. Make sure to set the ground layer of the map to "Ground" inorder for the police to traverse it
    */
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PoliceBehavior : MonoBehaviour
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

    /*  Detection  */
    [Header("Detection Settings")]
    public float sightDistance;
    public float reachDistance;
    private bool withinSight, withinReach;

    /*  Timers  */
    private bool alreadyChasing = false;
    public float sightCountdown = 2.0f; // Time for how long the player needs to stay in line-of-sight before the enemy starts chasing
    private float sightTimer = 0.0f;
    public float chaseDuration = 3.0f; // Time for how long the enemy will chase the player before giving up
    private float chaseTimer = 0.0f;

    private void Awake()
    {
        /*  Setting up variables    */
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        player = GameObject.Find("Player").transform;
    }

    void Update()
    {
        if (Physics.Raycast(transform.position, -transform.up, 2f, groundLayer))
        {
            DetectPlayer();
        }
        else
        {
            agent.SetDestination(player.position);
        }
    }

    private void DetectPlayer()
    {
        withinReach = Physics.CheckSphere(transform.position, reachDistance, playerLayer);
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
            chaseTimer = chaseDuration; // Reset chase timer when player is in sight
        }
        else
        {
            sightTimer = 0.0f;
            if (chaseTimer > 0)
            {
                chaseTimer -= Time.deltaTime;
            }
            else
            {
                alreadyChasing = false;
            }
        }

        if ((sightTimer >= sightCountdown || chaseTimer > 0 && !withinSight || alreadyChasing) && !withinReach)
        {
            // Chase
            alreadyChasing = true;
            ChasePlayer();
        }

        else if (withinSight && withinReach)
        {
            chaseTimer = 0;
            // If within reach, attack or something
            AttackPlayer();
        }
        else
        {
            // Idle
            PathingDefault();
        }

    }

    private void ChasePlayer()
    {
        // Debug.Log("Chasing");
        agent.SetDestination(player.position);
        transform.LookAt(player);
    }

    private void AttackPlayer()
    {
        // Debug.Log("Attacking");
        agent.SetDestination(transform.position);
        transform.LookAt(player);
        GameManager.Instance.SetGameState(GameManager.GameState.Over);
    }

    private void PathingDefault()
    {
        if (!walkPointExist) FindWalkPoint();

        if (walkPointExist)
        {
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
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
            walkPointExist = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, reachDistance);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightDistance);
    }

}
