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

    public float cooldownBeforeWalking = 1.0f; // Time before the NPC starts walking again after being stunned

    /*  Detection  */
    [Header("Detection Settings")]
    public float stunDuration;

    [SerializeField] NPCDetection npcDetection;

    private void Awake()
    {
        if (anim != null)
            anim.SetBool("isWalking", true);

        /*  Setting up variables    */
        agent = GetComponent<NavMeshAgent>();
        agent.speed = agentDefaultSpeed;
        player = GameObject.Find("Player").transform;

        if (stunDuration <= 0f)
            stunDuration = 2.0f; // default stun duration

        /*  NPC Detection Event Listeners  */
        npcDetection.PlayerNoticed += SmoothLookAt; // notice = look at player
        npcDetection.PlayerRecognized += Runaway; // recognize = run away
        npcDetection.PlayerStartLost += PlayerLost; // couldn't find player = player lost
        npcDetection.PlayerCompleteLost += PathingDefault; // completely lost player = normal behavior
    }

    void Update()
    {
        SetAnimationState(agent.velocity.magnitude > 0.1f);
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

    private void PlayerLost()
    {
        Debug.Log("NPC player lost");
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

    void SmoothLookAt(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
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

}
