using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class HealthController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] float maxHealth = 100;
    float current;

    bool alive = true;
    public UnityEvent<DamageInfo> OnDamaged;
    public UnityEvent OnDeath;

    void Awake() => current = maxHealth;

    public void TakeDamage(DamageInfo dmg)
    {
        current -= dmg.amount;
        OnDamaged?.Invoke(dmg);
        // Debug.Log($"{gameObject.name} took {dmg.amount} damage. Current health: {current}");
        if (alive && current <= 0)
        {
            alive = false;
            current = 0;
            OnDeath?.Invoke();
        }
    }

    public void Revive() 
    {
        alive = true;
        current = maxHealth;
        Debug.Log("Revived! Health restored to " + maxHealth);
    }

}
public struct DamageInfo
{
    public float amount;
    public Vector3 hitPoint;
    public Vector3 force;

    public DamageInfo(float amt, Vector3 point, Vector3 impulse)
    {
        amount = amt;
        hitPoint = point;
        force = impulse;
    }
}