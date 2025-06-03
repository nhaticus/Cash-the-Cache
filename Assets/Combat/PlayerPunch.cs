using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPunch : MonoBehaviour
{
    [SerializeField] private Hitbox punchHitbox;
    [SerializeField] private float punchDuration = 0.5f;
    [SerializeField] private Animator fistAnimator;
    [SerializeField] private GameObject vrArms;
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
        // press punch button and not already punching
        if ((UserInput.Instance && UserInput.Instance.Punch) || (UserInput.Instance == null && Input.GetMouseButtonDown(1)) && !isPunching) 
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
