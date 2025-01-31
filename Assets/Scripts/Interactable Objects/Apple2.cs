using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple2 : MonoBehaviour, InteractEvent
{
    public void Interact()
    {
        GetComponent<Rigidbody>().AddForce(new Vector3(0, 30, 00));
    }
}
