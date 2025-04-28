using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class AnchoredRotation : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 3;

    [Header("Dependencies")]
    [SerializeField] Transform AnchorPoint;
    [SerializeField] float RotationRadius;

    float rotAmount = 0;
    float angle = 0;

    private void Start() {
        rotAmount = 2 * Mathf.PI * (speed / 3) * Time.deltaTime;
    }

    public void SetRotationAmount(float speed) {
        this.speed = speed;
        rotAmount = 2 * Mathf.PI * (speed / 3) * Time.deltaTime;
    }
    
    void Update()
    {
        angle = (angle + rotAmount) % (2 * Mathf.PI);

        Vector2 newPos = transform.position;

        newPos.y = AnchorPoint.position.y + (int)Mathf.Round(RotationRadius * Mathf.Sin(angle));
        newPos.x = AnchorPoint.position.x + (int)Mathf.Round(RotationRadius * Mathf.Cos(angle));
        transform.position = newPos;
    }
    
}
