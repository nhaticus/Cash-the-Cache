// Video used https://www.youtube.com/watch?v=DU-yminXEX0
// Controls the driving behavior of the car using Unity's WheelCollider physics.
// Handles player input for steering, acceleration, and braking, and syncs visual
// wheel models with the physics wheels.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private float horizontalInput, verticalInput;
    private float currentSteerAngle, currentbreakForce;
    private bool isBreaking;

    // Settings
    [SerializeField] private float motorForce, breakForce, maxSteerAngle;

    // Wheel Colliders
    [SerializeField] private WheelCollider frontLeftWheelCollider, frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider, rearRightWheelCollider;

    // Wheels
    [SerializeField] private Transform frontLeftWheelTransform, frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform, rearRightWheelTransform;
    private Rigidbody rb;

    [SerializeField] SingleAudio singleAudio;

    private void OnEnable()
    {
        Ticker.OnTickAction += Tick;
    }

    private void OnDisable()
    {
        Ticker.OnTickAction -= Tick;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.5f, 0); // Lower center of mass (adjust Y as needed)
    }

    private void FixedUpdate()
    {
        MyInput();
        HandleMotor();
        HandleSteering();


        float currentSpeedMPH = GetCurrentSpeedMPH();
        Debug.Log($"Current Speed: {currentSpeedMPH:F1} MPH");
    }

    private void Tick()
    {
        UpdateWheels();
    }

    private void MyInput()
    {
        if (UserInput.Instance)
        {
            verticalInput = UserInput.Instance.Move.y;
            horizontalInput = UserInput.Instance.Move.x;
            isBreaking = UserInput.Instance.Inventory;

            if (UserInput.Instance.Punch)
                singleAudio.PlaySFX("car horn");
        }
        else
        {
            verticalInput = Input.GetAxis("Vertical");
            horizontalInput = Input.GetAxis("Horizontal");
            isBreaking = Input.GetKey(KeyCode.Space);

            if (Input.GetMouseButtonDown(1))
                singleAudio.PlaySFX("car horn");
        }
    }

    private void HandleMotor()
    {
        float currentSpeedMPH = GetCurrentSpeedMPH();

        // Ease in: less force at higher speed
        float accelerationMultiplier = 0.5f;




        // CAP max speed ? no force if over 120
        if (currentSpeedMPH >= 120f && verticalInput > 0f)
        {
            frontLeftWheelCollider.motorTorque = 0f;
            frontRightWheelCollider.motorTorque = 0f;
        }
        else
        {
            frontLeftWheelCollider.motorTorque = verticalInput * motorForce * accelerationMultiplier;
            frontRightWheelCollider.motorTorque = verticalInput * motorForce * accelerationMultiplier;
        }

        // Brakes
        currentbreakForce = isBreaking ? breakForce : 0f;
        ApplyBreaking();
    }

    private void ApplyBreaking()
    {
        frontRightWheelCollider.brakeTorque = currentbreakForce;
        frontLeftWheelCollider.brakeTorque = currentbreakForce;
        rearLeftWheelCollider.brakeTorque = currentbreakForce;
        rearRightWheelCollider.brakeTorque = currentbreakForce;
    }

    private void HandleSteering()
    {
        currentSteerAngle = maxSteerAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }

    public float GetCurrentSpeedMPH()
    {
        return rb.velocity.magnitude * 2.23694f; // Converts from m/s to mph
    }

}
