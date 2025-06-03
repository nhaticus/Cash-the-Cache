using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GenericNPC : MonoBehaviour
{
    

    #region State Machine
    public NPCStateManager stateMachine { get; set; }

    public NPCPatrolState patrolState { get; set; }
    public NPCLookAtState lookAtState { get; set; }
    public NPCRunawayState runawayState { get; set; }
    #endregion

    #region Dependencies
    public Rigidbody rb;
    public TMP_Text reportText;
    #endregion

    private void Awake()
    {
        stateMachine = new NPCStateManager();
        patrolState = new NPCPatrolState(this, stateMachine);
        lookAtState = new NPCLookAtState(this, stateMachine);
        runawayState = new NPCRunawayState(this, stateMachine);
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        stateMachine.Initialize(patrolState);
    }


    void Update()
    {
        stateMachine.currentState.FrameUpdate();
    }

    private void FixedUpdate()
    {
        stateMachine.currentState.PhysicsUpdate();
    }



}
