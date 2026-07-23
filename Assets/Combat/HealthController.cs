using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthController : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 50;
    float currentHealth;
    float invincibilityTime = 0.15f, currentInvincibility = 0;

    bool alive = true, canDamage = true;
    public UnityEvent<DamageInfo> OnDamaged;
    public UnityEvent OnDeath;

    [Header("Sounds")]
    [SerializeField] SingleAudio singleAudio;
    [SerializeField] string[] hurtSFX;

    void Awake() => currentHealth = maxHealth;

    public void TakeDamage(DamageInfo dmg)
    {
        if (canDamage == false)
            return;

        StartCoroutine(SetInvincible());

        currentHealth -= dmg.amount;
        OnDamaged?.Invoke(dmg);
        //Debug.Log($"{gameObject.name} took {dmg.amount} damage. Current health: {currentHealth}");
        if (alive && currentHealth <= 0)
        {
            alive = false;
            currentHealth = 0;
            OnDeath?.Invoke();
        }
        else // play hurt sound if not dead
        {
            if (hurtSFX.Length > 0)
                singleAudio.PlaySFX(hurtSFX[Random.Range(0, hurtSFX.Length)]);
        }
    }

    IEnumerator SetInvincible()
    {
        canDamage = false;
        currentInvincibility = invincibilityTime;
        while(currentInvincibility > 0)
        {
            currentInvincibility -= Time.deltaTime;
            yield return null;
        }
        canDamage = true;
    }

    public void Revive() 
    {
        alive = true;
        currentHealth = maxHealth;
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