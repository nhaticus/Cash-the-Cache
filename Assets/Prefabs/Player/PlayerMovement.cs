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

    [Header("Crouch Settings")]
    public Transform cameraHolder;
    public float crouchSpeed = 2f;
    private float originalSpeed;
    private Vector3 standingCamPos;
    private Vector3 crouchingCamPos;
    private bool isCrouching = false;

    public Transform orientation;

    public bool touchingWall;

    [SerializeField] SingleAudio singleAudio;
    private bool isPlayingFootsteps = false;

    float horizontalInput, verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;
    Transform tf;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        tf = GetComponent<Transform>();
        rb.freezeRotation = true;

        touchingWall = false;

        if (PlayerManager.Instance)
        {
            originalSpeed = PlayerManager.Instance.getMoveSpeed();
            moveSpeed = PlayerManager.Instance.getMoveSpeed();
        }

        standingCamPos = new Vector3(0f, 1f, 0f);
        crouchingCamPos = new Vector3(0f, 0.5f, 0f);
    }

    private void Update()
    {
        rb.drag = groundDrag;
        MyInput();
        HandleCrouch();

        SpeedControl();

        HandleFootstepSound();
        Vector3 targetCamPos = isCrouching ? crouchingCamPos : standingCamPos;
        cameraHolder.localPosition = Vector3.Lerp(cameraHolder.localPosition, targetCamPos, Time.deltaTime * 10f);

    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        if (UserInput.Instance)
        {
            verticalInput = UserInput.Instance.Move.y;
            horizontalInput = UserInput.Instance.Move.x;
        }
        else
        {
            verticalInput = Input.GetAxis("Vertical");
            horizontalInput = Input.GetAxis("Horizontal");
        }
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
        if (Time.timeScale <= 0 && isPlayingFootsteps) // prevent footstep sound when paused
        {
            singleAudio.StopSFX();
            isPlayingFootsteps = false;
            return;
        }
        // Check if player is moving horizontally
        bool isMoving = rb.velocity.magnitude > 0.1f;
        if (isMoving && !isPlayingFootsteps) {
            singleAudio.PlaySFX("footsteps", true);
            isPlayingFootsteps = true;
        }
        else if (!isMoving && isPlayingFootsteps) {
            singleAudio.StopSFX();
            isPlayingFootsteps = false;
        }
    }

    // Resets orientation of the player to face original rotation. This causes camera to also reset position.
    public void resetOrientation()
    {
        orientation.eulerAngles = new Vector3(0f, 0f, 0f);
        tf.eulerAngles = new Vector3(0f, 0f, 0f);
    }

    private void HandleCrouch()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (!isCrouching)
            {
                PlayerManager.Instance.setMoveSpeed(crouchSpeed);
                isCrouching = true;
            }
        }
        else
        {
            if (isCrouching)
            {
                PlayerManager.Instance.setMoveSpeed(originalSpeed);
                isCrouching = false;
            }
        }
    }

}
