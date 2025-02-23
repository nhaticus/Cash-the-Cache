using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*
 * Script attached to a player hurtbox
 * Defines the player health and what happens when they are hurt
 */

public class PlayerHealth : MonoBehaviour
{
    public int health = 1;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish"))
        {
            OnDamage(1);
        }
    }

    [HideInInspector] public UnityEvent Death;
    void OnDamage(int damage)
    {
        health -= damage;

        if(health <= 0)
        {
            Debug.Log("dead");
            PlayerManager.Instance.setMoveSpeed(0);
            PlayerManager.Instance.ableToInteract = false;

            Death.Invoke(); // send dead signal for GameOver
        }
    }
}
