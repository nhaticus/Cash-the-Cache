using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPunch : MonoBehaviour
{
    [SerializeField] private Hitbox punchHitbox;
    [SerializeField] private float punchDuration = 0.2f;
    [SerializeField] private Animator fistAnimator;

    Collider punchCollider;
    private bool isPunching = false;
    private bool isLeftPunch = true;

    void Start()
    {
        // cache the collider and disable it
        punchCollider = punchHitbox.GetComponent<Collider>();
        punchCollider.enabled = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isPunching) 
        { 
            StartCoroutine(Punch());
        }
    }

    IEnumerator Punch()
    {
        isPunching = true;                 //set the punching state to true

        if (isLeftPunch)
        {
            fistAnimator.SetTrigger("PunchLeft");
            yield return new WaitForSeconds(.5f);
        }
        else 
        { 
            fistAnimator.SetTrigger("PunchRight");
            yield return new WaitForSeconds(.5f);
        }


        isLeftPunch = !isLeftPunch;
        Debug.Log("Punching!");
        punchCollider.enabled = true;                //enable the trigger

        yield return new WaitForSeconds(punchDuration);
        punchCollider.enabled = false;               //disable it again
        isPunching = false;                //reset the punching state
    }
}
