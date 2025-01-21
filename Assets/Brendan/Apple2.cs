using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple2 : MonoBehaviour, InteractEvent
{
    public void Interact()
    {
        Debug.Log("Apple 2 stuff");
        GetComponent<Rigidbody>().AddForce(new Vector3(30, 10, 20));
    }
}
