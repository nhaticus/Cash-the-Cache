using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInteract : MonoBehaviour
{
    GameObject objRef;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && objRef != null)
        {
            ExecuteEvents.Execute<InteractEvent>(objRef, null, (x, y) => x.Interact());
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        objRef = other.gameObject;
    }

    private void OnTriggerExit(Collider other)
    {
        objRef = null;
    }
}
