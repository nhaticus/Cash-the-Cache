using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public abstract class NPC : MonoBehaviour
{
    protected NavMeshAgent agent;
    [SerializeField] protected Animator anim;
    protected Transform player;

    [Header("Debug Settings")]
    public bool debugMode = false;

    [Header("Navmesh Agent Settings")]
    public float agentDefaultSpeed = 3.5f;

    [Header("Layers for Detection")]
    public LayerMask playerLayer;
    public LayerMask groundLayer;

    [Header("Patrolling Settings")]
    [SerializeField] protected Vector3 walkPoint;
    protected bool walkPointExist;
    public float walkPointRange = 10f;

    [Header("Detection Settings")]
    public float sightDistance = 10f;
    public int sightAngle = 60;
    public float cooldownBeforeWalking = 1.0f;
    public float stunDuration = 1.0f;

    [Header("Timers")]
    public float sightCountdown = 2.0f;
    protected float sightTimer = 0.0f;

    protected bool withinSight = false;
    protected bool isStunned = false;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = agentDefaultSpeed;
        player = GameObject.Find("Player").transform;
    }

    protected virtual void Update()
    {
        if (isStunned) return;

        SetAnimationState(agent.velocity.magnitude > 0.5f);
        HandleBehavior();
    }

    protected abstract void HandleBehavior();

    protected bool IsPlayerInSight()
    {
        if (Vector3.Distance(transform.position, player.position) > sightDistance)
        {
            withinSight = false;
        }

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if (angleToPlayer <= sightAngle)
        {
            if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, sightDistance))
            {
                if (((1 << hit.collider.gameObject.layer) & playerLayer) != 0)
                {
                    withinSight = true;
                    SmoothLookAt(player.position);
                    agent.SetDestination(transform.position);
                    // sightTimer += Time.deltaTime;
                    return true;
                }
            }
        }
        // sightTimer = Mathf.Max(0, sightTimer - Time.deltaTime);
        return false;
    }

    protected void SmoothLookAt(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 3f);
    }

    protected void FindWalkPoint()
    {
        for (int attempts = 0; attempts < 10; attempts++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * walkPointRange + transform.position;
            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit navHit, walkPointRange, NavMesh.AllAreas))
            {
                NavMeshPath path = new NavMeshPath();
                if (agent.CalculatePath(navHit.position, path) && path.status == NavMeshPathStatus.PathComplete)
                {
                    walkPoint = navHit.position;
                    walkPointExist = true;
                    return;
                }
            }
        }
    }

    protected void SetAnimationState(bool isWalking)
    {
        if (anim != null)
        {
            anim.SetBool("isWalking", isWalking);
        }
    }

    protected IEnumerator StunCoroutine(float duration)
    {
        isStunned = true;
        agent.isStopped = true;
        yield return new WaitForSeconds(duration);
        agent.isStopped = false;
        isStunned = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bat"))
        {
            StartCoroutine(StunCoroutine(stunDuration));
        }
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
