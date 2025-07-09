/*  This script is attached to Police prefab
    * FUNCTIONALITY: Police will be spawned outside the map through generator script and will path towards the player until they reach the house, 
    then they will start randomly walking around until they run into the player and chase them
    * HOW TO USE: Drag the prefab into any script and instantiate it. Make sure to set the ground layer of the map to "Ground" inorder for the police to traverse it
    */
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum PoliceState
{
    Patrol,
    LookAt,
    Chase,
    Attack
}

public class PoliceBehavior : MonoBehaviour
{
    NavMeshAgent agent;
    [SerializeField] Animator anim;
    Transform player;

    [Header("Debug Settings")]
    public bool debugMode = false;

    /*  Navmesh Agent Settings   */
    [Header("Navmesh Agent Settings")]
    public float speed = 6.5f;

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
    [SerializeField] private bool withinSight, withinReach;

    /*  Timers  */
    private bool alreadyChasing = false;
    public float sightCountdown = 2.0f; // Time for how long the player needs to stay in line-of-sight before the enemy starts chasing
    private float sightTimer = 0.0f;
    public float chaseDuration = 3.0f; // Time for how long the enemy will chase the player before giving up
    private float chaseTimer = 0.0f;

    [Header("State Machine")]
    PoliceState currentState = PoliceState.Chase; // currently only state is chase because police know where player is already

    private void Awake()
    {
        // increase health and speed based on difficulty
        speed += PlayerPrefs.GetInt("Difficulty") / 2.5f;
        GetComponent<HealthController>().maxHealth += Mathf.Floor((PlayerPrefs.GetInt("Difficulty") * 1.4f) + (0.15f * DataSystem.Data.gameState.currentReplay));
        speed = Mathf.Min(speed, 12); // max value is 12

        /*  Setting up variables    */
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;

        player = GameObject.Find("Player").transform;

        SetChasePlayer();
    }

    void Update()
    {
        StateUpdate(); 
    }

    void StateUpdate()
    {
        // only states now are run after player and attack
        if(currentState == PoliceState.Chase)
        {
            ChasePlayer();
        }
    }

    // unused
    private void DetectPlayer()
    {
        // detect for player in 45 degree angle
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
        withinReach = Physics.CheckSphere(transform.position, reachDistance, playerLayer);

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
            AttackPlayer();
        }
        else
        {
            PathingDefault();
        }
    }

    public void SetChasePlayer()
    {
        currentState = PoliceState.Chase;
        agent.SetDestination(player.position);
        SetAnimationState("isRunning", true);
    }

    void ChasePlayer()
    {
        agent.SetDestination(player.position);
        SmoothLookAt(player.position);
    }

    private void AttackPlayer()
    {
        currentState = PoliceState.Attack;
        agent.SetDestination(transform.position);
    }

    private void PathingDefault()
    {
        if (!walkPointExist)
            FindWalkPoint();
        else
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
        {
            walkPointExist = false;
            StartCoroutine(WaitBeforeMoving());
        }
    }

    void SmoothLookAt(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 3f);
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

    private void SetAnimationState(string animation, bool value)
    {
        if (anim == null) return;
        anim.SetBool(animation, value);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, reachDistance);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightDistance);
    }
}
