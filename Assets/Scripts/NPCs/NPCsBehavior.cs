/*  This script is attached to NPC prefab
    * FUNCTIONALITY: NPC will wander around until detecting the player, after which it will run away to a designated exit point
    * HOW TO USE: drag the prefab into and set the exit point attached to the parent object to any entrance/exit point. Make sure to set the ground layer of the map to "Ground" inorder for the NPC to traverse it
    */
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class NPCsBehavior : MonoBehaviour
{
    private NavMeshAgent agent;

    private Transform player;

    /*  Navmesh Agent Settings   */
    [Header("Navmesh Agent Settings")]
    public float agentDefaultSpeed = 3.5f; // default speed 3.5f

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
    public float stunDuration;
    public float sightDistance;
    private bool withinSight;
    private bool detectedPlayer = false;

 

    [Header("Detection Settings")]
    /*  Timers  */
    public float sightCountdown = 2.0f; // Time for how long the player needs to stay in line-of-sight before the enemy starts chasing
    private float sightTimer = 0.0f;
    private void Awake()
    {
        /*  Setting up variables    */
        agent = GetComponent<NavMeshAgent>();
        agent.speed = agentDefaultSpeed;
        player = GameObject.Find("Player").transform;
        if (stunDuration == 0f)
        {
            stunDuration = 1.0f; // default stun duration
        }
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
        for (int i = -45; i <= 45; i += 5)
        {
            Vector3 direction = Quaternion.Euler(0, i, 0) * transform.forward;
            Debug.DrawRay(transform.position, direction * sightDistance, Color.red);
            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, sightDistance))
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("playerLayer"))
                {
                    withinSight = true;
                    break;
                }
                else
                {
                    withinSight = false;
                }
            }
        }

        // Debug.Log("Within Sight: " + withinSight + " Within Reach: " + withinReach);

        if (withinSight)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 3f);
            agent.SetDestination(transform.position);
            sightTimer += Time.deltaTime;
        }
        else
        {
            sightTimer = 0.0f;
            PathingDefault();
        }

        if (sightTimer >= sightCountdown)
        {
            detectedPlayer = true;
        }

    }


    private void Runaway()
    {
        agent.SetDestination(exit.transform.position);
        Vector3 distanceToExit = transform.position - exit.position;
        if (agent.speed == agentDefaultSpeed)
        {
            agent.speed *= 1.5f;
        }

        if (distanceToExit.magnitude < 2.0f)
        {

            Destroy(transform.parent.gameObject);
            GameManager.Instance.NPCLeaving();
            // Debug.Log("NPC has escaped!");
        }

        // Debug.Log("Player is in sight! RUN!");
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

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Bat"))
        {
            Debug.Log("Stunned!");
            StartCoroutine(Stun());
        }
    }

    private IEnumerator Stun()
    {
        agent.isStopped = true;
        yield return new WaitForSeconds(stunDuration);
        agent.isStopped = false;
    }

    public float GetDetectionRatio()
    {
        // Avoid division by zero if sightCountdown = 0
        return (sightCountdown > 0f) ? (sightTimer / sightCountdown) : 0f;
    }
}
