using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCRunawayState : NPCBaseState
{
    public override void EnterState(NPCStateManager manager)
    {
        manager.report.text = "run away";
    }

    public override void UpdateState(NPCStateManager manager)
    {

    }
}
