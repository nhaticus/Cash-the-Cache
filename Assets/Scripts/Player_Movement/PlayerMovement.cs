using System;
using Unity.VisualScripting;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{

    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;

    [Header("Jump")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;
    private float jumpBoost = 1.0f;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    [Header("Keybinds")]
    public
     KeyCode jumpKey = KeyCode.Space;

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

        ResetJump();
        grounded = true;
        touchingWall = false;

        if (playerCameraScript)
        {
            playerCameraScript.sens = PlayerPrefs.GetFloat("Sensitivity", 120);
        }
    }

    private void Update()
    {
        // ground check 
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

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



        // when to jump
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        //AudioManager.Instance.PlaySFX("footstep_sound");

        // calculate movement Direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else if (!grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }



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
        {
            if (AudioManager.Instance)
                AudioManager.Instance.StopSFX("footstep_sound");
            return;
        }

        // Check if player is moving horizontally
        bool isMoving = rb.velocity.magnitude > 0.1f;
        if (isMoving && !isPlayingFootsteps) {
            //Debug.Log("playing footstep sound");
            if (AudioManager.Instance)
                AudioManager.Instance.PlaySFX("footstep_sound", true);
            isPlayingFootsteps = true;
        }
        else if (!isMoving && isPlayingFootsteps) {
            //Debug.Log("stopping footstep sound");
            if (AudioManager.Instance)
                AudioManager.Instance.StopSFX("footstep_sound");
            isPlayingFootsteps = false;
        }
    }


    private void Jump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce * jumpBoost, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }


    // Resets orientation of the player to face original rotation. This causes camera to also reset position.
    public void resetOrientation()
    {
        orientation.eulerAngles = new Vector3(0f, 0f, 0f);
        tf.eulerAngles = new Vector3(0f, 0f, 0f);
    }

 
}


