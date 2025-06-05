using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]          // this is the attack?range trigger
public class NPCPunch : MonoBehaviour
{
    [Header("Hitbox")]
    [SerializeField] private Hitbox punchHitbox;    // the fist hitbox that really does the damage
    [SerializeField] private float punchDuration = 0.5f;

    [Header("Timing")]
    [SerializeField] private float attackRate = 1.0f;   // seconds between punches

    [Header("Animation")]
    [SerializeField] private Animator fistAnimator;

    private Collider punchCollider;
    private bool isLeftPunch = true;
    private bool isPunching;
    private float nextAllowedTime;                     // cooldown timer

    private void Awake()
    {
        punchCollider = punchHitbox.GetComponent<Collider>();
        punchCollider.enabled = false;                // never leave it live!

        // the trigger we’re sitting on must be a Trigger
        var range = GetComponent<Collider>();
        range.isTrigger = true;
    }

    /* ?????????????????????????  SENSE PLAYER  ????????????????????????? */

    private void OnTriggerStay(Collider other)
    {
        // 1) must be a Hurtbox
        if (other.gameObject.layer != LayerMask.NameToLayer("Hurtbox")) return;

        // 2) make sure it actually belongs to the player (not another cop rag?doll etc.)
        Hurtbox hb = other.GetComponent<Hurtbox>();
        if (hb == null || hb.owner == null || !hb.owner.CompareTag("Player")) return;

        
        // 3) only start if we’re allowed to punch again
        if (!isPunching && Time.time >= nextAllowedTime)
        {
            StartCoroutine(PunchRoutine());
        }
    }

    /* ?????????????????????????  PUNCH LOGIC  ????????????????????????? */

    private IEnumerator PunchRoutine()
    {
        isPunching = true;
        nextAllowedTime = Time.time + attackRate;      // set next punch time

        /* pick side & play animation */
        if (isLeftPunch) fistAnimator.SetTrigger("PunchLeft");
        else fistAnimator.SetTrigger("PunchRight");
        isLeftPunch = !isLeftPunch;

        yield return new WaitForSeconds(0.5f);         // wind?up matches your anim

        punchCollider.enabled = true;                  // damage window open
        yield return new WaitForSeconds(punchDuration);
        punchCollider.enabled = false;                 // damage window closed

        isPunching = false;
    }
}
