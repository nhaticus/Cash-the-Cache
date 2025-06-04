using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPunch : MonoBehaviour
{
    [Header("Hitbox")]
    [SerializeField] private Hitbox punchHitbox;
    [SerializeField] private float punchDuration = 0.5f;

    [Header("Animation")]
    [SerializeField] private Animator fistAnimator;
    [SerializeField] private GameObject vrArms;

    PlayerMovement playerMovement;

    Collider punchCollider;
    private bool isPunching = false;
    private bool isLeftPunch = true;

    void Start()
    {
        // cache the collider and disable it
        punchCollider = punchHitbox.GetComponent<Collider>();
        punchCollider.enabled = false;

        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        // press punch button, not already punching, and can punch
        if ((UserInput.Instance && UserInput.Instance.Punch) || (UserInput.Instance == null && Input.GetMouseButtonDown(1))
            && !isPunching && playerMovement.canMove) 
        { 
            StartCoroutine(Punch());
        }
    }

    IEnumerator Punch()
    {
        isPunching = true;                 //set the punching state to true
        //vrArms.SetActive(true);
        
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
       // Debug.Log("Punching!");
        punchCollider.enabled = true;                //enable the trigger


        yield return new WaitForSeconds(punchDuration);

       // vrArms.SetActive(false);
        punchCollider.enabled = false;               //disable it again
        isPunching = false;                //reset the punching state
    }
}
