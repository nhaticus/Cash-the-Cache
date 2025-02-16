using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour, InteractEvent
{
    [SerializeField] GameObject[] obj;
    [SerializeField] int difficulty = 3;

    [SerializeField] GameObject canvas;

    private void Start()
    {
        canvas.SetActive(false);
    }
    public void Interact()
    {
        canvas.SetActive(true);
        /* create a bunch of screws
         * connect screw event to an integer
         * when integer is 0: OpenBox()
         */
    }

    void OpenBox()
    {
        canvas.SetActive(false);
        Instantiate(obj[Random.Range(0, obj.Length)], transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
