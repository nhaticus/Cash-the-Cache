using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{
    private NavMeshAgent agent;

    private Transform player;

    /*  Layers for detection    */
    public LayerMask playerLayer;

    /*  States  */
    public float sightDistance, reachDistance;
    private bool withinSight, withinReach;


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
        transform.LookAt(player);
    }

    private void PathingDefault()
    {
        agent.SetDestination(transform.position);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, reachDistance);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightDistance);
    }

}
