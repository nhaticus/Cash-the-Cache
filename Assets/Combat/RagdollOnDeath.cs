using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollOnDeath : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private RagdollController ragdollController;

    [Header("Launch Settings")]
    [Tooltip("Upward force applied when dying")]
    [SerializeField] private float launchStrength = 50f;

    private HealthController health;

    void OnEnable()
    {
        health = GetComponent<HealthController>();
        health.OnDeath += HandleDeath;
    }

    void OnDisable()
    {
        health.OnDeath -= HandleDeath;
    }

    private void HandleDeath()
    {
        Debug.Log("?? RagdollOnDeath.HandleDeath() fired");
        // disable root animation/AI/etc without null-conditional assignment
        if (TryGetComponent<Animator>(out var anim))
            anim.enabled = false;

        if (TryGetComponent<UnityEngine.AI.NavMeshAgent>(out var agent))
            agent.enabled = false;

        if (TryGetComponent<Collider>(out var rootCol))
            rootCol.enabled = false;

        // make sure these types actually exist and match your scripts!
        if (TryGetComponent<NPCsBehavior>(out var behavior))
            behavior.enabled = false;

    
        // Pass an upward impulse into the ragdoll call
        Vector3 impulse = Vector3.up * launchStrength;
        ragdollController.SetRagdoll(true, impulse);
    }
}
