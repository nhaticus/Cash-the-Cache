using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Hit Data")]
public class HitData : ScriptableObject
{
    public float baseDamage = 10;
    public Vector3 knockback = Vector3.forward * 4;
}
