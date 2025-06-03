using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/*
 * State Machine only for home owners
 */

public class NPCStateManager : MonoBehaviour
{
    NPCBaseState currentState;

    public NPCPatrolState patrolState = new NPCPatrolState();
    public NPCLookAtState lookAtState = new NPCLookAtState();
    public NPCRunawayState runawayState = new NPCRunawayState();

    public TMP_Text report;

    public NPCDetection detection;

    void Start()
    {
        SwitchState(patrolState);
    }

    void Update()
    {
        currentState.UpdateState(this);
    }

    public void SwitchState(NPCBaseState state)
    {
        currentState = state;
        state.EnterState(this);
    }
}
