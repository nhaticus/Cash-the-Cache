using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NPCsBehavior : MonoBehaviour
{
    NavMeshAgent agent;
    [SerializeField] Animator anim;
    Transform player;

    [Header("Debug Settings")]
    public bool debugMode = false;

    /*  Navmesh Agent Settings   */
    [Header("Navmesh Agent Settings")]
    public float agentDefaultSpeed; // default speed 3.5f

    /*  Layers for detection    */
    [Header("Layers for Detection")]
    public LayerMask playerLayer;
    public LayerMask groundLayer;

    /*  Random walking  */
    [Header("Patrolling Settings")]
    [SerializeField] Vector3 walkPoint;
    bool walkPointExist;
    public float walkPointRange;

    /*  Detection  */
    [Header("Detection Settings")]
    public float stunDuration;
    public float sightDistance;
    public int sightAngle; // Angle of the detection cone
    public float cooldownBeforeWalking = 1.0f; // Time before the NPC starts walking again after being stunned
    [SerializeField] bool withinSight;
    [SerializeField] bool detectedPlayer = false;

    /*  Timers  */
    public float sightCountdown = 2.0f; // Time for how long the player needs to stay in line-of-sight before the enemy starts chasing
    float sightTimer = 0.0f;
    private void Awake()
    {
        if (anim != null)
        {
            anim.SetBool("isWalking", true);
        }
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
        SetAnimationState(agent.velocity.magnitude > 0.1f);
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
        if (Vector3.Distance(transform.position, player.position) > sightDistance)
        {
            withinSight = false;
        }
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if (angleToPlayer <= sightAngle)
        {
            if (debugMode)
            {
                Debug.DrawRay(transform.position, directionToPlayer * sightDistance, Color.red);
            }
            if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, sightDistance))
            {
                if (((1 << hit.collider.gameObject.layer) & playerLayer) != 0)
                {
                    withinSight = true;
                }
            }
        }
        else
        {
            withinSight = false;
        }
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
            sightTimer = Mathf.Max(0, sightTimer - Time.deltaTime);
            PathingDefault();
        }

        if (sightTimer >= sightCountdown)
        {
            detectedPlayer = true;
        }

    }


    private void Runaway()
    {
        Vector3 exit = GameManager.Instance.GetNPCExitPoint();
        agent.SetDestination(exit);
        Vector3 distanceToExit = transform.position - exit;
        if (agent.speed == agentDefaultSpeed)
        {
            agent.speed *= 1.5f;
        }

        if (distanceToExit.magnitude < 2.0f)
        {

            Destroy(gameObject);
            GameManager.Instance.NPCLeaving();
        }

    }

    private void PathingDefault()
    {
        if (!walkPointExist) FindWalkPoint();

        else
        {
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1.0f)
        {
            walkPointExist = false;
            StartCoroutine(WaitBeforeMoving(cooldownBeforeWalking));
        }
    }

    private IEnumerator WaitBeforeMoving(float time)
    {
        agent.isStopped = true;
        yield return new WaitForSeconds(time);
        agent.isStopped = false;
    }


    private void FindWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        Vector3 rayOrigin = new Vector3(walkPoint.x, walkPoint.y + 2f, walkPoint.z);
        if (Physics.Raycast(rayOrigin, Vector3.down, 4f, groundLayer))
        {
            NavMeshPath path = new NavMeshPath();
            if (agent.CalculatePath(walkPoint, path) && path.status == NavMeshPathStatus.PathComplete)
            {
                walkPointExist = true;
            }
        }
    }

    private void SetAnimationState(bool isWalking)
    {
        if (anim == null) return;
        anim.SetBool("isWalking", isWalking);
    }

    public float GetDetectionRatio()
    {
        // Avoid division by zero if sightCountdown = 0
        float percentage = (sightCountdown > 0f) ? Mathf.Clamp01(sightTimer / sightCountdown) : 0f;
        if (debugMode)
        {
            Debug.Log("Detection Ratio: " + percentage);
        }
        return percentage;

    }
    /*
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Bat"))
        {
            if (debugMode)
            {
                Debug.Log("Detected Bat!");
            }
            Stun();
        }
    }
    */

    public void Stun()
    {
        Debug.Log("NPC starting Wait before moving coroutine");
        StartCoroutine(WaitBeforeMoving(stunDuration));
    }
    private void OnDrawGizmosSelected()
    {
        if (debugMode)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, sightDistance);

            Vector3 leftLimit = Quaternion.Euler(0, -sightAngle, 0) * transform.forward;
            Vector3 rightLimit = Quaternion.Euler(0, sightAngle, 0) * transform.forward;
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, leftLimit * sightDistance);
            Gizmos.DrawRay(transform.position, rightLimit * sightDistance);
        }
    }
}
