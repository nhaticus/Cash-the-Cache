using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// item info copy
[CreateAssetMenu]
public class ItemInfo : ScriptableObject
{
    [SerializeField] GameObject prefab;
    public GameObject Prefab => prefab;

    public Sprite sprite;
    public string itemName;
    public int value;
    public int weight;
}
