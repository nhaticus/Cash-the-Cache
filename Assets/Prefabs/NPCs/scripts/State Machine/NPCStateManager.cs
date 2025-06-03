using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCStateManager : MonoBehaviour
{
    NPCBaseState currentState;

    NPCPatrolState patrolState = new NPCPatrolState();
    NPCLookAtState lookAtState = new NPCLookAtState();
    NPCRunawayState runawayState = new NPCRunawayState();

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
