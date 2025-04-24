using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CopCarAIController : MonoBehaviour
{
    [Header("NavMesh Target to Follow")]
    [SerializeField] private Transform navTarget;


    [Header("Wheel Colliders")]
    [SerializeField] private WheelCollider frontLeft, frontRight, rearLeft, rearRight;

    [Header("Wheel Transforms")]
    [SerializeField] private Transform frontLeftTransform, frontRightTransform, rearLeftTransform, rearRightTransform;

    [Header("Driving Settings")]
    [SerializeField] private float maxSteerAngle = 20f;
    [SerializeField] private float motorForce = 1000f;
    [SerializeField] private float stopDistance = 3f;

    private Rigidbody rb;

    public void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 toTarget = navTarget.position - transform.position;
        Vector3 flatDir = new Vector3(toTarget.x, 0, toTarget.z).normalized;
        float distance = toTarget.magnitude;
        float speed = rb.velocity.magnitude;

        // Speed limit grows with distance
        float speedLimit = Mathf.Lerp(5f, 20f, distance / 30f);

        // Direction relative to car
        Vector3 localDir = transform.InverseTransformDirection(flatDir);
        float steer = Mathf.Clamp(localDir.x, -1f, 1f) * maxSteerAngle;

        frontLeft.steerAngle = steer;
        frontRight.steerAngle = steer;

        if (distance > stopDistance)
        {
            if (speed < speedLimit)
            {
                // Accelerate
                frontLeft.motorTorque = motorForce;
                frontRight.motorTorque = motorForce;
                ApplyBrakes(0f); // release brakes
            }
            else
            {
                // Coast + resist over-speeding
                frontLeft.motorTorque = 0f;
                frontRight.motorTorque = 0f;
                ApplyBrakes(300f); // gentle slow down
            }
        }
        else
        {
            // Too close — full stop
            frontLeft.motorTorque = 0f;
            frontRight.motorTorque = 0f;
            ApplyBrakes(1000f);
        }

        UpdateWheels();

    }



    private void ApplyBrakes(float amount)
    {
        frontLeft.brakeTorque = amount;
        frontRight.brakeTorque = amount;
        rearLeft.brakeTorque = amount;
        rearRight.brakeTorque = amount;
    }

    private void UpdateWheels()
    {
            // Real wheel pose from WheelColliders
            UpdateSingleWheel(frontLeft, frontLeftTransform);
            UpdateSingleWheel(frontRight, frontRightTransform);
            UpdateSingleWheel(rearLeft, rearLeftTransform);
            UpdateSingleWheel(rearRight, rearRightTransform);

    }

    private void UpdateSingleWheel(WheelCollider col, Transform trans)
    {
        col.GetWorldPose(out Vector3 pos, out Quaternion rot);
        trans.position = pos;
        trans.rotation = rot;
    }
}
