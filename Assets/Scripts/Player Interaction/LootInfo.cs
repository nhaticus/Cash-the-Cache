using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Scriptable Object that defines the loot data
[CreateAssetMenu(fileName = "Loot ScriptableObject")]
public class LootInfo : ScriptableObject
{
    [SerializeField] GameObject prefab;
    public GameObject Prefab => prefab;

    public Sprite sprite;
    public string itemName;
    public int value;
    public int weight;
}