using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Hurtbox : MonoBehaviour
{
    public HealthController owner;          // drag the root object once

    void Awake()
    {
        // make sure we only generate trigger overlaps, not physics pushes
        var col = GetComponent<Collider>();
        col.isTrigger = true;
        gameObject.layer = LayerMask.NameToLayer("Hurtbox");
    }
}
