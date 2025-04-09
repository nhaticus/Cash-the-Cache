using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DestroyOnInteract : MonoBehaviour, InteractEvent
{
    public void Interact()
    {
        Destroy(gameObject);
    }
}
