using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericArmoredVehiclePhysicsModel : MonoBehaviour
{
    [Header("Palettes")]
    public List<WheelCollider> RightPalette;
    public List<WheelCollider> LeftPalette;

    [Header("Motor")]
    public float Power = 1200E3F; // Watt
    public float RPM = 1500.0F;
    private float TorquePerWheel; // Nm
    
    [Header("Control Inputs")]
    [Range(0, 1)] public float ThrustInput = 0.0F;
    [Range(-1, 1)] public float SteerInput = 0.0F;
    
    private bool Breaking = false;

    [Header("Rigidbody Settings")]
    public Rigidbody Rigidbody;
    public float Mass = 60E3F; // Kg

    [Header("Steering")]
    public float SteeringThrustCoefficient = 800.0F;

    [Header("Brake")]
    public float BrakeTorque = 3000.0F; // Nm

    void Start()
    {
        SetMass();
        SetTorquePerWheel();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    public void SetThrust(float newThrust)
    {
        ThrustInput = newThrust;
    }

    public void SetSteering(float newSteering)
    {
        SteerInput = newSteering;
    }

    public void HandleMovement()
    {
        ApplyTorque();

        if (Breaking)
        {
            ApplyBrake();
        }
    }

    public void SetBrakeState(bool newState)
    {
        Breaking = newState;
    }

    private void ApplyTorque()
    {
        foreach (WheelCollider wheel in RightPalette)
        {
            wheel.motorTorque = EvaluateEffectiveRightTorque();
        }

        foreach (WheelCollider wheel in LeftPalette)
        {
            wheel.motorTorque = EvaluateEffectiveLeftTorque();
        }
    }

    private void ApplyBrake()
    {
        SetThrust(0.0F);

        foreach (WheelCollider wheel in RightPalette)
        {
            wheel.motorTorque = 0.0F;
            wheel.brakeTorque = BrakeTorque;
        }

        foreach (WheelCollider wheel in LeftPalette)
        {
            wheel.motorTorque = 0.0F;
            wheel.brakeTorque = BrakeTorque;
        }
    }

    private float EvaluateTotalTorque()
    {
        return (Power * 60.0F) / (2.0F * Mathf.PI * RPM);
    }

    private float EvaluateBaseTorque()
    {
        return ThrustInput * TorquePerWheel;
    }

    private float EvaluateSteeringTorque()
    {
        return SteerInput * ThrustInput * SteeringThrustCoefficient;
    }

    private float EvaluateEffectiveRightTorque()
    {
        return EvaluateBaseTorque() - EvaluateSteeringTorque();
    }

    private float EvaluateEffectiveLeftTorque()
    {
        return EvaluateBaseTorque() + EvaluateSteeringTorque();
    }

    private void SetTorquePerWheel()
    {
        TorquePerWheel = EvaluateTotalTorque() / RightPalette.Count;
    }

    private void SetMass()
    {
        if (!Rigidbody)
        {
            Rigidbody = gameObject.AddComponent<Rigidbody>();
        }

        Rigidbody.mass = Mass;
    }
}
