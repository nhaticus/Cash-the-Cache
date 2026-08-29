using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPunch : MonoBehaviour
{
    [Header("Hitbox")]
    [SerializeField] private Hitbox punchHitbox;
    [SerializeField] private float punchDuration = 0.25f;

    [Header("Animation")]
    [SerializeField] Animator leftArm;
    [SerializeField] Animator rightArm;

    Collider punchCollider;
    private bool isPunching = false;
    private bool isLeftPunch = true;

    void Start()
    {
        // disable hitbox
        punchCollider = punchHitbox.GetComponent<Collider>();
        punchCollider.enabled = false;
    }

    void Update()
    {
        // press punch button, not already punching, and can punch
        if (((UserInput.Instance && UserInput.Instance.Punch) || (UserInput.Instance == null && Input.GetMouseButtonDown(1)))
            && !isPunching && PlayerManager.Instance.ableToInteract) 
        {
            StartCoroutine(Punch());
        }
    }

    IEnumerator Punch()
    {
        isPunching = true;   // set the punching state to true
        
        if (isLeftPunch)
        {
            leftArm.SetTrigger("Punch");
            yield return new WaitForSeconds(0.1f);
        }
        else 
        {
            rightArm.SetTrigger("Punch");
            yield return new WaitForSeconds(0.1f);
        }

        isLeftPunch = !isLeftPunch;     // switch punching arm
        punchCollider.enabled = true;
        
        yield return new WaitForSeconds(punchDuration);

        punchCollider.enabled = false;  // reset the punching state
        isPunching = false;

        yield return new WaitForSeconds(0.2f); // delay to prevent constant punching
    }
}
