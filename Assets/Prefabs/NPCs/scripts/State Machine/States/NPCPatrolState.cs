using System.Collections;
using System.Collections.Generic;
using Unity.Services.Analytics;
using UnityEngine;
using UnityEngine.AI;

public class NPCPatrolState : NPCBaseState
{
    public NPCPatrolState(GenericNPC _npc, NPCStateManager _manager) : base(_npc, _manager)
    {
    }

    public override void EnterState()
    {
        base.EnterState();

        npc.reportText.text = "Patrol";
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
