using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum NPCState
{
    Patrol,
    LookAt,
    RunAway
}

public class NPCsBehavior : MonoBehaviour
{
    NavMeshAgent agent;
    [SerializeField] Animator anim;

    /*  Navmesh Agent Settings   */
    [Header("Navmesh Agent Settings")]
    [SerializeField] float agentDefaultSpeed = 3.5f;
    [SerializeField] float runningSpeed = 5;

    /*  Layers for detection    */
    [Header("Layers for Detection")]
    [SerializeField] LayerMask groundLayer;

    /*  Random walking  */
    [Header("Patrolling Settings")]
    [SerializeField] Vector3 walkPoint;
    bool walkPointExist;
    public float walkPointRange;

    public float cooldownBeforeWalking = 2.0f; // Time before the NPC starts walking again after being stunned

    GameObject objectToLookAt;

    [SerializeField] NPCState currentState = NPCState.Patrol;

    private void Awake()
    {
        if (anim != null)
            anim.SetBool("isWalking", true);

        /*  Setting up variables    */
        agent = GetComponent<NavMeshAgent>();
        agent.speed = agentDefaultSpeed;
    }

    void Update()
    {
        SetAnimationState(agent.velocity.magnitude > 0.1f);

        StateUpdate();
    }

    void StateUpdate()
    {
        if(currentState == NPCState.Patrol)
        {
            PathingDefault();
        }
        else if (currentState == NPCState.LookAt)
        {
            SmoothLookAt(objectToLookAt);
        }
        else if(currentState == NPCState.RunAway)
        {
            // keep checking distance to exit
            Vector3 exit = GameManager.Instance.GetNPCExitPoint();

            Debug.Log("running to " + exit);

            Vector3 distanceToExit = transform.position - exit;
            if (distanceToExit.magnitude <= 3.0f)
            {
                // reached exit: send event and destroy NPC
                GameManager.Instance.NPCLeaving();
                Destroy(gameObject);
            }
        }
    }

    public void PathingDefault()
    {
        if (!walkPointExist)
            FindWalkPoint();
        else
            agent.SetDestination(walkPoint);

        // reached destination, wait a little
        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude < 1.0f)
        {
            walkPointExist = false;
            StartCoroutine(WaitBeforeMoving(cooldownBeforeWalking));
        }
    }

    public void SetLookAt(GameObject player)
    {
        currentState = NPCState.LookAt;
        objectToLookAt = player;
        agent.isStopped = true;
    }

    public void SmoothLookAt(GameObject obj)
    {
        Vector3 direction = (obj.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
    }

    public void PlayerLost()
    {
        // wait a little
        StartCoroutine(WaitBeforeMoving(cooldownBeforeWalking));

        // go back to patrolling
        currentState = NPCState.Patrol;
        objectToLookAt = null;
    }

    public void SetRunaway()
    {
        // change state
        currentState = NPCState.RunAway;

        // get exit point destination
        Vector3 exit = GameManager.Instance.GetNPCExitPoint();
        agent.SetDestination(exit);
        agent.speed = runningSpeed;
    }

    /// <summary>
    /// Stops NPC for amount of time
    /// </summary>
    /// <param name="time"></param>
    private IEnumerator WaitBeforeMoving(float time)
    {
        Debug.Log("wait no moving");
        agent.isStopped = true;
        yield return new WaitForSeconds(time);
        if (agent.isStopped)
            Debug.Log("yup its stopped");
        agent.isStopped = false;
    }

    /// <summary>
    /// Gets the NPC a new destination to walk to
    /// </summary>
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
