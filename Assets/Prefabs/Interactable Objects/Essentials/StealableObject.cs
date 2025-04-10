using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// the script to attach to the game object
public class StealableObject : MonoBehaviour
{
    public LootInfo lootInfo;

    public void SetInfo(LootInfo info)
    {
        lootInfo = info;
    }
}
