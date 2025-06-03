using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCLookAtState : NPCBaseState
{
    public override void EnterState(NPCStateManager manager)
    {
        Debug.Log("switch to look at");
        manager.report.text = "look at";
        //manager.detection.PlayerRecognized.AddListener(); // player recognized = run away
        // manager.detection.PlayerLost.AddListener(manager.SwitchState(manager.patrolState));
    }

    public override void UpdateState(NPCStateManager manager)
    {

    }
    /*
    public void SmoothLookAt(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
    }
    */
}
