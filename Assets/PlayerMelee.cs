using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMelee : MonoBehaviour
{

    [Header("Attacking")]
    public float attackCooldown = 0.1f;
    public float attackSpeed = 1f;
    public int attackDamage = 1;
    public LayerMask attackLayer;
    public BoxCollider hitbox;

    bool attacking = false;
    bool readyToAttack = true;
    int attackCount;

    public GameObject cam;

    private void Awake()
    {
        cam = GameObject.Find("Main Camera");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Attack();
        }
    }

    public void Attack()
    {
        if (!readyToAttack || attacking) return;

        readyToAttack = false;
        attacking = true;

        Invoke(nameof(ResetAttack), attackSpeed);
        Invoke(nameof(AttackHitbox), attackCooldown);


    }

    void ResetAttack()
    {
        attacking = false;
        readyToAttack = true;
        hitbox.enabled = false;
    }

    void AttackHitbox()
    {
        hitbox.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.tag);
        if (other.gameObject.tag == "AI")
        {
            Debug.Log("Hit NPC");
            NPCsBehavior hurtbox = other.gameObject.GetComponent<NPCsBehavior>();

            hurtbox.Stun();
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(transform.position, hitbox.size);
       
    }


}
