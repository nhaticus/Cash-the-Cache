using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mapbox.Unity.Utilities;

public class HealthController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] float maxHealth = 100;
    float current;

    public Action<DamageInfo> OnDamaged;
    public Action OnDeath;

    void Awake() => current = maxHealth;

    public void TakeDamage(DamageInfo dmg)
    {
        current -= dmg.amount;
        OnDamaged?.Invoke(dmg);
        Debug.Log($"Took {dmg.amount} damage. Current health: {current}");
        if (current <= 0)
        {
            current = 0;
            OnDeath?.Invoke();
        }
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