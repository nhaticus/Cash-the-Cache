using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockoutOnDeath : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] RagdollController ragdollController;
    [SerializeField] HealthController healthController;

    [Header("KO Settings")]
    [Tooltip("Seconds to stay ragdolled")]
    [SerializeField] float knockoutDuration = 10f;
    [SerializeField] bool permaDeath = false;

    [Header("Launch Settings")]
    [Tooltip("Upward force applied when knocked")]
    const float airForce = 0.4f;
    [SerializeField] private float launchStrength = 50f;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private Transform pelvisBone;

    void OnEnable()
    {
        healthController.OnDeath.AddListener(HandleKnockout);
    }

    void OnDisable()
    {
        healthController.OnDeath.RemoveListener(HandleKnockout);
    }

    private void HandleKnockout()
    {
        StartCoroutine(KnockoutRoutine());
    }

    private IEnumerator KnockoutRoutine()
    {
        HitMarker.Instance?.ShowKnock();

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

        // get camera
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
        // launch away from camera
        Vector3 forward;
        if (camTransform != null)
            forward = camTransform.forward;
        else
            forward = Vector3.forward;

        // launch body
        Vector3 launchDir = forward + Vector3.up * airForce;
        launchDir.Normalize();
        Vector3 impulse = launchDir * launchStrength;
        ragdollController.SetRagdoll(true, impulse);

        // 3) Wait out the KO timer
        yield return new WaitForSeconds(knockoutDuration);

        //    - Revive
        if (!permaDeath)
        {
            // 4) “Get up”:
            //    - Turn off ragdoll
            //    - Reset root position to pelvis (optional but often needed)
            Vector3 finalPos = pelvisBone.position;
            transform.position = finalPos;
            ragdollController.SetRagdoll(false);

            healthController.Revive();

            //    - Re-enable your root systems
            anim.enabled = true;
            agent.enabled = true;
            rootCol.enabled = true;
            behavior.enabled = true;
        }
        
    }
}
