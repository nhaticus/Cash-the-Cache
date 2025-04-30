using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMelee : MonoBehaviour
{

    [Header("Attacking")]
    public float attackRange = 3f;
    public float attackCooldown = 0.5f;
    public float attackSpeed = 1f;
    public int attackDamage = 1;
    public LayerMask attackLayer;

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

        Debug.Log("Attacking");

        Invoke(nameof(ResetAttack), attackSpeed);
        Invoke(nameof(AttackRaycast), attackCooldown);


    }

    void ResetAttack()
    {
        attacking = false;
        readyToAttack = true;
    }

    void AttackRaycast()
    {
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, attackRange, attackLayer))
        {
            Debug.Log("Hit!!!");
        }
    }
}
