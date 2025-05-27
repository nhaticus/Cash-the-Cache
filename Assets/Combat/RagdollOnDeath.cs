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

    [SerializeField] private Transform playerCamera;

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

        Transform camTransform = null;
        if (playerCamera != null)
        {
            camTransform = playerCamera;
        }
        else
        {
            Camera mainCam = Camera.main;
            if (mainCam != null)
                camTransform = mainCam.transform;
        }



        Vector3 forward;
        if (camTransform != null)
            forward = camTransform.forward;
        else
            forward = Vector3.forward;

        Vector3 launchDir = forward + Vector3.up * 0.3f;
        launchDir.Normalize();

       
        Vector3 impulse = launchDir * launchStrength;
        ragdollController.SetRagdoll(true, impulse);
    }
}

