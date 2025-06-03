using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/*
 * State Machine for all NPCs
 */

public class NPCStateManager : MonoBehaviour
{
    public NPCBaseState currentState { get; set; }

    public void Initialize(NPCBaseState state)
    {
        currentState = state;
        currentState.EnterState();
    }

    public void SwitchState(NPCBaseState state)
    {
        currentState.ExitState();
        currentState = state;
        currentState.EnterState();
    }

}
