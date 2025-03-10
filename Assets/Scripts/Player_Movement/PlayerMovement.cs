using System;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;

    [Header("Camera")]
    public PlayerCam playerCameraScript;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;
    Transform tf;

    public bool touchingWall;
    private bool isPlayingFootsteps = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        tf = GetComponent<Transform>();
        rb.freezeRotation = true;

        touchingWall = false;

        if (playerCameraScript)
        {
            playerCameraScript.sens = PlayerPrefs.GetFloat("Sensitivity", 120);
        }
    }

    private void Update()
    {
        rb.drag = groundDrag;
        MyInput();
        SpeedControl();

        HandleFootstepSound();
    
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {

        verticalInput = Input.GetAxisRaw("Vertical");
        horizontalInput = Input.GetAxisRaw("Horizontal");
    }

    private void MovePlayer()
    {
        // calculate movement Direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void HandleFootstepSound()
    {
        if (Time.timeScale <= 0) // prevent sound when paused
        { // WORKS but it will continue to try to stop sfx so might not be efficient
            if (AudioManager.Instance)
                AudioManager.Instance.StopSFX("footstep_sound");
            return;
        }

        // Check if player is moving horizontally
        bool isMoving = rb.velocity.magnitude > 0.1f;
        if (isMoving && !isPlayingFootsteps) {
            if (AudioManager.Instance)
                AudioManager.Instance.PlaySFX("footstep_sound", true);
            isPlayingFootsteps = true;
        }
        else if (!isMoving && isPlayingFootsteps) {
            if (AudioManager.Instance)
                AudioManager.Instance.StopSFX("footstep_sound");
            isPlayingFootsteps = false;
        }
    }

    // Resets orientation of the player to face original rotation. This causes camera to also reset position.
    public void resetOrientation()
    {
        orientation.eulerAngles = new Vector3(0f, 0f, 0f);
        tf.eulerAngles = new Vector3(0f, 0f, 0f);
    }

}
