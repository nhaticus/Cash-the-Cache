using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct LootInfo
{
    public string name;
    public int value;
    public int weight;

    public LootInfo(string _name, int _value, int _weight)
    {
        name = _name;
        value = _value;
        weight = _weight;
    }
}
    

public class StealableObject : MonoBehaviour
{
    [SerializeField] string name = "";
    [SerializeField] int value = 0;
    [SerializeField] int weight = 0;

    public LootInfo lootInfo;

    private void Start()
    {
        lootInfo = new LootInfo(name, value, weight);
    }
}
