using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveFlashlight : MonoBehaviour
{
    public Transform flashlightPosition;

    private void Update()
    {
        transform.position = flashlightPosition.position;
    }
}
