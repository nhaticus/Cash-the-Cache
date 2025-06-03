using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCPatrolState : NPCBaseState
{
    NPCStateManager manager;
    public override void EnterState(NPCStateManager manager)
    {
        manager.report.text = "patroling";
        manager.detection.PlayerNoticed.AddListener(StartLookAt); // player noticed = look at
    }

    public override void UpdateState(NPCStateManager manager)
    {

    }

    void StartLookAt(GameObject player)
    {
        Debug.Log("try look at");
        manager.SwitchState(manager.lookAtState);
    }
}
