using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class Hitbox : MonoBehaviour
{
    public HitData hitData;           // pick the ScriptableObject
    public GameObject attackerRoot;   // drag the player or enemy root
    public UnityEvent hitEvent;

    void Awake()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
        gameObject.layer = LayerMask.NameToLayer("Hitbox");
    }

    void OnTriggerEnter(Collider other)
    {
        // ignore self?hits
        if (other.transform.IsChildOf(attackerRoot.transform)) return;

        var hurt = other.GetComponentInChildren<Hurtbox>();
        if (hurt == null) return;

        var health = hurt.owner;
        if (health == null) return;
        //Debug.Log($"Hitbox overlapped {other.name} (layer={LayerMask.LayerToName(other.gameObject.layer)})");

        SuccessHit(other, health);
    }

    void SuccessHit(Collider other, HealthController health)
    {
        hitEvent.Invoke();

        // build the DamageInfo the HealthController expects
        Vector3 dir = (other.transform.position - transform.position).normalized;
        var info = new DamageInfo(hitData.baseDamage,
                                  transform.position,
                                  dir * hitData.knockback.magnitude);

        health.TakeDamage(info);
    }
}
