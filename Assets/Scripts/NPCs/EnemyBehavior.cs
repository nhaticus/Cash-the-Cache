using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{
    private NavMeshAgent agent;

    private Transform player;

    /*  Layers for detection    */
    public LayerMask playerLayer, groundLayer;

    /*  Random walking  */
    public Vector3 walkPoint;
    bool walkPointExist;
    public float walkPointRange;

    /*  States  */
    public float sightDistance, reachDistance;
    private bool withinSight, withinReach;

    /*  Timers  */
    private bool alreadyChasing = false;
    public float sightDuration = 2.0f; // Time for how long the player needs to stay in line-of-sight before the enemy starts chasing
    private float sightTimer = 0.0f;
    public float chaseDuration = 3.0f; // Time for how long the enemy will chase the player before giving up
    private float chaseTimer = 0.0f;

    private void Awake()
    {
        /*  Setting up variables    */
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player").transform;
    }

    void Update()
    {
        DetectPlayer(2);
    }

    private void DetectPlayer(int option)
    {
        if (option == 1)
        {
            withinSight = Physics.CheckSphere(transform.position, sightDistance, playerLayer);
            withinReach = Physics.CheckSphere(transform.position, reachDistance, playerLayer);

            if (withinSight && !withinReach)
            {
                // Chase
                ChasePlayer();
            }
            else if (withinSight && withinReach)
            {
                // If within reach, attack or something
                AttackPlayer();
            }
            else
            {
                // Idle
                PathingDefault();
            }
        }
        else if (option == 2)
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

            if ((sightTimer >= sightDuration || chaseTimer > 0 && !withinSight || alreadyChasing) && !withinReach)
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
        }
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
