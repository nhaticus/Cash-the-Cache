using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;


    private void Awake()
    {
        // setting the variable without manual work
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("PlayerObj").transform;
    }

    void Update()
    {
        float OPTION = 2;
        RaycastHit hit;
        Vector3 direction = player.position - transform.position;
        /*  OPTION 1: Have a raycast from enemy to player to avoid obstacles
            If the raycast hits the player and they are within a certain distance, the enemy will move towards the player
        */
        if (OPTION == 1){
            if (Physics.Raycast(transform.position, direction, out hit))
            {
                Debug.DrawRay(transform.position, direction, Color.red);
                if (hit.transform == player)
                {
                    float distance = Vector3.Distance(player.position, transform.position);
                    if (distance < 10.0f) // replace 10.0f with your desired distance
                    {
                        agent.SetDestination(player.position);
                    }
                    else
                    {
                        agent.ResetPath();
                    }
                }
                else
                {
                    agent.ResetPath();
                }
            }
            else
            {
                agent.ResetPath();
            }
        } 
        /*  OPTION 2: raycast in a 45-degree cone in front of the enemy
            If the raycast hits the player and they are within a certain distance, the enemy will move towards the player
        */
        if (OPTION == 2){
            float angle = Vector3.Angle(transform.forward, player.position - transform.position);
            if (angle < 45.0f)
            {
                if (Physics.Raycast(transform.position, direction, out hit))
                {
                    Debug.DrawRay(transform.position, direction, Color.red);
                    if (hit.transform == player)
                    {
                        float distance = Vector3.Distance(player.position, transform.position);
                        if (distance < 10.0f) // replace 10.0f with your desired distance
                        {
                            agent.SetDestination(player.position);
                        }
                        else
                        {
                            agent.ResetPath();
                        }
                    }
                    else
                    {
                        agent.ResetPath();
                    }
                }
                else
                {
                    agent.ResetPath();
                }
            }
            else
            {
                agent.ResetPath();
            }
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("touching");
        }
    }
}
