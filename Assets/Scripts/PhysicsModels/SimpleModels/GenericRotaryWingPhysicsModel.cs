using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericRotaryWingPhysicsModel : MonoBehaviour
{
    [Header("Main Rotor")]
    public Transform MainRotor;
    public float RotorThrust = 40000.0f; // N
    public float RotorRPM = 500.0f;      // RPM

    [Header("Tail Rotor")]
    public Transform TailRotor;
    public float TailRotorThrust = 5000.0f; // N

    [Header("Control Inputs")]
    [Range(-1, 1)] public float CollectiveInput = 0.0f;
    [Range(-1, 1)] public float CyclicPitchInput = 0.0f;
    [Range(-1, 1)] public float CyclicRollInput = 0.0f;
    [Range(-1, 1)] public float YawInput = 0.0f;

    [Header("Rigidbody Settings")]
    public Rigidbody Rigidbody;
    public float Mass = 8000.0f; // Kg

    [Header("Rotor Settings")]
    public float RotorLiftCoefficient = 1.0f;
    public float CyclicControlCoefficient = 500.0f;

    [Header("Aerodynamics")]
    public float DragCoefficient = 0.1f;

    void Start()
    {
        SetMass();
    }

    void FixedUpdate()
    {
        HandleMovement();
        RotateRotors();
        ApplyAerodynamicDrag();
    }

    private void HandleMovement()
    {
        ApplyLift();
        ApplyCyclicControls();
        ApplyYawControl();
    }

    private void ApplyLift()
    {
        float liftForce = CollectiveInput * RotorThrust * RotorLiftCoefficient;
        Rigidbody.AddForce(Vector3.up * liftForce);
    }

    private void ApplyCyclicControls()
    {
        float pitchForce = CyclicPitchInput * CyclicControlCoefficient;
        Rigidbody.AddForce(transform.forward * pitchForce);

        float rollForce = CyclicRollInput * CyclicControlCoefficient;
        Rigidbody.AddForce(transform.right * rollForce);
    }

    private void ApplyYawControl()
    {
        float yawTorque = YawInput * TailRotorThrust;
        Rigidbody.AddTorque(Vector3.up * yawTorque);
    }

    private void RotateRotors()
    {
        if (MainRotor)
        {
            MainRotor.Rotate(Vector3.up, RotorRPM * Time.fixedDeltaTime * 6.0f);
        }

        if (TailRotor)
        {
            TailRotor.Rotate(Vector3.right, RotorRPM * Time.fixedDeltaTime * 8.0f);
        }
    }

    private void ApplyAerodynamicDrag()
    {
        Rigidbody.drag = DragCoefficient;
    }

    private void SetMass()
    {
        if (!Rigidbody)
        {
            Rigidbody = gameObject.AddComponent<Rigidbody>();
        }

        Rigidbody.mass = Mass;
    }

    public void SetCollectiveInput(float input)
    {
        CollectiveInput = Mathf.Clamp(input, -1.0f, 1.0f);
    }

    public void SetCyclicPitchInput(float input)
    {
        CyclicPitchInput = Mathf.Clamp(input, -1.0f, 1.0f);
    }

    public void SetCyclicRollInput(float input)
    {
        CyclicRollInput = Mathf.Clamp(input, -1.0f, 1.0f);
    }

    public void SetYawInput(float input)
    {
        YawInput = Mathf.Clamp(input, -1.0f, 1.0f);
    }
}
