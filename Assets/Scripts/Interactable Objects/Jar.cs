using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jar : MonoBehaviour, InteractEvent
{
    [SerializeField] GameObject[] obj;
    public void Interact()
    {
        Instantiate(obj[Random.Range(0, obj.Length - 1)], transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
