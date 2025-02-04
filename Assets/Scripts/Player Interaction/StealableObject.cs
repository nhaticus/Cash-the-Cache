using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct LootInfo
{
    public GameObject prefab;
    public Sprite sprite;
    public string name;
    public int value;
    public int weight;

    public LootInfo(GameObject _prefab, Sprite _sprite, string _name, int _value, int _weight)
    {
        prefab = _prefab;
        sprite = _sprite;
        name = _name;
        value = _value;
        weight = _weight;
    }

    public LootInfo(LootInfo _lootInfo)
    {
        prefab = _lootInfo.prefab;
        sprite = _lootInfo.sprite;
        name = _lootInfo.name;
        value = _lootInfo.value;
        weight = _lootInfo.weight;
    }
}
    

public class StealableObject : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] Sprite sprite;
    [SerializeField] string objectName = "";
    [SerializeField] int value = 0;
    [SerializeField] int weight = 0;

    public LootInfo lootInfo;

    private void Start()
    {
        lootInfo = new LootInfo(prefab, sprite, objectName, value, weight);
    }
}
