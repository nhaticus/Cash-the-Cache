using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Scriptable Object that defines the loot data
[CreateAssetMenu(fileName = "Loot ScriptableObject")]
public class LootInfo : ScriptableObject
{
    public GameObject prefab;
    public Sprite sprite;
    public string itemName;
    public int value;
    public int weight;
}

// the script to attach to the game object
public class StealableObject : MonoBehaviour
{
    public LootInfo lootInfo;
}
