using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Apple : MonoBehaviour, InteractEvent
{
    public void Interact()
    {
        Destroy(gameObject);
    }
}
