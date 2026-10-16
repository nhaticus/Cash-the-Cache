using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageIndicator : MonoBehaviour
{
    [SerializeField] HealthController playerHealth;

    float currHealth, totalHealth = 100;
    [SerializeField] float firstIndicatePercentage = 65, secondIndicatePercentage = 45, thirdIndicatePercentage = 30;
    [SerializeField] GameObject firstIndicator, secondIndicator, thirdIndicator;
    Coroutine firstShake, secondShake, thirdShake; // keep track of coroutines to turn off

    private void Start()
    {
        totalHealth = playerHealth.maxHealth;
        currHealth = playerHealth.currentHealth;

        firstIndicator.SetActive(false);
        secondIndicator.SetActive(false);
        thirdIndicator.SetActive(false);

        OnHit(); // determine if should be shaking, mostly for testing
    }

    public void OnHit()
    {
        currHealth = playerHealth.currentHealth;

        float healthPercentage = (currHealth / totalHealth) * 100;

        if (!firstIndicator.activeSelf && healthPercentage <= firstIndicatePercentage)
        {
            firstIndicator.SetActive(true);
            firstShake = StartCoroutine(ShakeObject(firstIndicator, 4));
        }
        if (!secondIndicator.activeSelf && healthPercentage <= secondIndicatePercentage)
        {
            secondIndicator.SetActive(true);
            secondShake = StartCoroutine(ShakeObject(secondIndicator, 6));
        }
        if (!thirdIndicator.activeSelf && healthPercentage <= thirdIndicatePercentage)
        {
            thirdIndicator.SetActive(true);
            thirdShake = StartCoroutine(ShakeObject(thirdIndicator, 10));
        }

        // Shouldn't happen, but just in case you heal back to normal percentages
        if (firstIndicator.activeSelf && healthPercentage >= firstIndicatePercentage)
        {
            StopCoroutine(firstShake);
            firstIndicator.SetActive(false);
        }
        if (secondIndicator.activeSelf && healthPercentage >= secondIndicatePercentage)
        {
            StopCoroutine(secondShake);
            secondIndicator.SetActive(false);
        }
        if (thirdIndicator.activeSelf && healthPercentage >= thirdIndicatePercentage)
        {
            StopCoroutine(thirdShake);
            thirdIndicator.SetActive(false);
        }
    }

    IEnumerator ShakeObject(GameObject obj, float magnitude)
    {
        Vector2 originalPos = obj.transform.position;

        while (true)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            // Smoothly move towards the random shake point
            obj.transform.position = new Vector2(originalPos.x + x, originalPos.y + y);

            yield return null;
        }
    }
}
