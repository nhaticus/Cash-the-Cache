using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    [SerializeField] bool startRagdolled = false;
    Animator anim;
    List<Rigidbody> bodies = new();
    List<Collider> cols = new();

    void Awake()
    {
        anim = GetComponent<Animator>();

        foreach (var rb in GetComponentsInChildren<Rigidbody>(true))
        {
            if (rb.gameObject == gameObject) continue;
            bodies.Add(rb);
            cols.Add(rb.GetComponent<Collider>());
        }

        SetRagdoll(startRagdolled);
    }

    public void SetRagdoll(bool on, Vector3 impulse = default)
    {
        anim.enabled = !on;

        for (int i = 0; i < bodies.Count; i++)
        {
            bodies[i].isKinematic = !on;
            cols[i].isTrigger = !on;
            bodies[i].detectCollisions = on;

            if (on && impulse != Vector3.zero)
                bodies[i].AddForce(impulse, ForceMode.Impulse);
        }
    }
}
