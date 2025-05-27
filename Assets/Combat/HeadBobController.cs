using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBobController : MonoBehaviour
{

    [Header("Head-Bob Settings")]
    public bool enableBob = true;
    [Range(0f, 0.1f)] public float amplitude = 0.015f;
    [Range(0f, 30f)] public float frequency = 10f;
    public float toggleSpeed = 3f;

    [Header("References")]
    [Tooltip("Drag your **Main Camera** here")]
    public Transform cameraTransform;
    [Tooltip("Your player Rigidbody")]
    public Rigidbody rb;

    [Header("Ground Check")]
    [Tooltip("Layers you walk on")]
    public LayerMask groundLayers;
    [Tooltip("? half your capsule height")]
    public float groundCheckDist = 1.2f;

    private Vector3 startLocalPos;
    private float bobTimer = 0f;

    void Awake()
    {
        if (cameraTransform == null) Debug.LogError("Assign Main Camera!");
        if (rb == null) rb = GetComponent<Rigidbody>();
        startLocalPos = cameraTransform.localPosition;
    }

    void Update()
    {
        if (!enableBob) return;

        // 1) see if we're grounded
        bool isGrounded = Physics.Raycast(
            transform.position + Vector3.up * 0.1f,
            Vector3.down,
            groundCheckDist + 0.1f,
            groundLayers
        );

        // 2) horizontal speed
        Vector3 flatVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        float speed = flatVel.magnitude;

        // 3) bob or reset
        if (isGrounded && speed > toggleSpeed)
        {
            bobTimer += Time.deltaTime * frequency;
            float y = Mathf.Sin(bobTimer) * amplitude;
            float x = Mathf.Cos(bobTimer * 0.5f) * amplitude * 0.5f;
            cameraTransform.localPosition = startLocalPos + new Vector3(x, y, 0f);
        }
        else
        {
            cameraTransform.localPosition = Vector3.Lerp(
                cameraTransform.localPosition,
                startLocalPos,
                Time.deltaTime * frequency
            );
        }
    }

}
