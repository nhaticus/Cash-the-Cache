using Mapbox.Unity.MeshGeneration.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPunch : MonoBehaviour
{
    [SerializeField] private Hitbox punchHitbox;
    [SerializeField] private float punchDuration = 0.2f;

    Collider punchCollider;

    void Start()
    {
        // cache the collider and disable it
        punchCollider = punchHitbox.GetComponent<Collider>();
        punchCollider.enabled = false;
    }

    void Update()
    {
        if ((UserInput.Instance && UserInput.Instance.Punch) || (UserInput.Instance == null && Input.GetMouseButtonDown(1)))
            StartCoroutine(Punch());
    }

    IEnumerator Punch()
    {
        // Debug.Log("Punching!");
        punchCollider.enabled = true;                // ? enable the trigger
        yield return new WaitForSeconds(punchDuration);
        punchCollider.enabled = false;               // ? disable it again
    }
}
