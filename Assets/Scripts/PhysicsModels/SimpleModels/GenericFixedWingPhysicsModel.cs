using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericFixedWingPhysicsModel : MonoBehaviour
{
    [Header("Engine Settings")]
    public float EngineThrust = 50000.0f; // N
    [Range(0, 1)] public float ThrustInput = 0.0f;

    [Header("Control Inputs")]
    [Range(-1, 1)] public float PitchInput = 0.0f;
    [Range(-1, 1)] public float RollInput = 0.0f;
    [Range(-1, 1)] public float YawInput = 0.0f;

    [Header("Rigidbody Settings")]
    public Rigidbody Rigidbody;
    public float Mass = 10000.0f; // Kg

    [Header("Aerodynamics")]
    public float LiftCoefficient = 1.5f;
    public float DragCoefficient = 0.05f;
    public float WingArea = 30.0f; // m²

    [Header("Control Surface Coefficients")]
    public float PitchForceCoefficient = 2000.0f;
    public float RollForceCoefficient = 1500.0f;
    public float YawForceCoefficient = 1000.0f;

    [Header("Gravity Settings")]
    public float Gravity = 9.81f;

    void Start()
    {
        SetMass();
    }

    void FixedUpdate()
    {
        ApplyThrust();
        ApplyLift();
        ApplyDrag();
        ApplyControlSurfaces();
    }

    private void ApplyThrust()
    {
        Vector3 thrustForce = transform.forward * ThrustInput * EngineThrust;
        Rigidbody.AddForce(thrustForce);
    }

    private void ApplyLift()
    {
        float airDensity = 1.225f;
        float velocitySquared = Rigidbody.velocity.sqrMagnitude;
        float liftForceMagnitude = 0.5f * airDensity * velocitySquared * WingArea * LiftCoefficient;

        Rigidbody.AddForce(transform.up * liftForceMagnitude);
    }

    private void ApplyDrag()
    {
        // Drag force = 0.5 * AirDensity * Velocity² * DragCoefficient * WingArea
        float airDensity = 1.225f;
        float velocitySquared = Rigidbody.velocity.sqrMagnitude;
        float dragForceMagnitude = 0.5f * airDensity * velocitySquared * DragCoefficient * WingArea;

        Vector3 dragForce = -Rigidbody.velocity.normalized * dragForceMagnitude;
        Rigidbody.AddForce(dragForce);
    }

    private void ApplyControlSurfaces()
    {
        float pitchTorque = PitchInput * PitchForceCoefficient;
        Rigidbody.AddTorque(transform.right * pitchTorque);
        
        float rollTorque = RollInput * RollForceCoefficient;
        Rigidbody.AddTorque(transform.forward * -rollTorque);

        float yawTorque = YawInput * YawForceCoefficient;
        Rigidbody.AddTorque(transform.up * yawTorque);
    }

    private void SetMass()
    {
        if (!Rigidbody)
        {
            Rigidbody = gameObject.AddComponent<Rigidbody>();
        }

        Rigidbody.mass = Mass;
        Rigidbody.useGravity = true;
    }

    public void SetThrustInput(float input)
    {
        ThrustInput = Mathf.Clamp01(input);
    }

    public void SetPitchInput(float input)
    {
        PitchInput = Mathf.Clamp(input, -1.0f, 1.0f);
    }

    public void SetRollInput(float input)
    {
        RollInput = Mathf.Clamp(input, -1.0f, 1.0f);
    }

    public void SetYawInput(float input)
    {
        YawInput = Mathf.Clamp(input, -1.0f, 1.0f);
    }
}
