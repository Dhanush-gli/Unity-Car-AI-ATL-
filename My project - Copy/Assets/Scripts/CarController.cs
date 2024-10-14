using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    private float horizontalInput;
    private float verticalInput;
    private float currentSteerAngle;
    private float currentbreakForce;
    private bool isBreaking;

    [SerializeField] private float motorForce;
    [SerializeField] private float breakForce;
    [SerializeField] private float maxSteerAngle;

    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider;
    [SerializeField] private WheelCollider rearRightWheelCollider;

    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheeTransform;
    [SerializeField] private Transform rearLeftWheelTransform;
    [SerializeField] private Transform rearRightWheelTransform;

    //Code for ai

    [SerializeField] private Transform targetPositionTransform;

    private Vector3 target_position;

    private CarDriverAi carDriverAI;

    private float forward_amount = 0f;
    private float turning_amount = 0f;

    private void FixedUpdate()
    {
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();

        

       

    }

   


    private void GetInput()
    {
        horizontalInput = Input.GetAxis(HORIZONTAL);
        verticalInput = Input.GetAxis(VERTICAL);
        isBreaking = Input.GetKey(KeyCode.Space);
    }

    private void HandleMotor()
    {
        frontLeftWheelCollider.motorTorque = forward_amount * motorForce;
        frontRightWheelCollider.motorTorque = forward_amount * motorForce;
        
    }

    

    private void ApplyBreaking()
    {
      
    }

    private void HandleSteering()
    {
        currentSteerAngle = maxSteerAngle * turning_amount;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheeTransform);
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

    private void Update()
    {
        //code for ai
        setTargetPosition(targetPositionTransform.position);

        Vector3 dirToMovePosition = (target_position - transform.position).normalized;
        float dot = Vector3.Dot(transform.forward, dirToMovePosition);
        float distanceToTarget = Vector3.Distance(transform.position, target_position);


        float angleToDir = Vector3.SignedAngle(transform.forward, dirToMovePosition, Vector3.up);

        float angletodirnormal = (angleToDir / 180) * 1.3f;



        //Debug.Log(turning_amount);

        if (distanceToTarget > 50)
        {

            motorForce = 5000;

            if (dot > 0f)
            {
                forward_amount = 1f;
            }
            else
            {
                float reverse_distance = 10f;
                if (distanceToTarget > reverse_distance)
                {
                    forward_amount = 1f;
                }
                else { forward_amount = -1f; }
                
            }

            turning_amount = angletodirnormal;
        }
        else if (distanceToTarget < 50f & distanceToTarget > 30f) 
        {
            if (dot > 0f)
            {
                forward_amount = 0.5f;
                
            }
            else
            {
                forward_amount = -0.5f;
                motorForce = 500;
            }

            turning_amount = angletodirnormal;
        }
        else if (distanceToTarget < 30f & distanceToTarget > 7f)
        {
            if (dot > 0f)
            {
                forward_amount = 0.3f;
                motorForce = 300;
            }
            else
            {
                forward_amount = -0.3f;
                motorForce = 300;
            }

            turning_amount = angletodirnormal;
        }
        else
        {
            Debug.Log("STOP!");
            frontRightWheelCollider.brakeTorque = 6000;
            frontLeftWheelCollider.brakeTorque = 6000;
            rearLeftWheelCollider.brakeTorque = 6000;
            rearRightWheelCollider.brakeTorque = 6000;

            forward_amount = 0f;
            turning_amount = 0f;

            ApplyBreaking();
        }


    }

   

    public void setTargetPosition(Vector3 targetPosition) 
    {
        this.target_position = targetPosition;
    }
}