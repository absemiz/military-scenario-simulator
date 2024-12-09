using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericInfantryPhysicsModel : MonoBehaviour
{
    [Header("Movement Settings")]
    public float MaxMoveSpeed = 5.0f; // m/s
    public float Acceleration = 20.0f; // m/s²
    public float Deceleration = 15.0f; // m/s²
    public float TurnSpeed = 45.0F; // °/s

    [Header("Jump Settings")]
    public float JumpForce = 7.0f; // N
    public float Gravity = 9.81f; // m/s²
    public float GroundCheckDistance = 0.2f; // m

    [Tooltip("Layer mask for ground detection.")]
    public LayerMask GroundMask;

    [Header("Rigidbody Settings")]
    public Rigidbody Rigidbody;

    private Vector3 CurrentVelocity;
    private bool IsGrounded;

    [Header("Control Inputs")]
    [Range(-1, 1)] public float MoveInput = 0.0f;
    [Range(-1, 1)] public float StrafeInput = 0.0f;
    [Range(-1, 1)] public float TurnInput = 0.0f;
    public bool JumpInput = false;

    private Transform GroundCheck;

    void Start()
    {
        SetRigidbody();
        CreateGroundCheck();
    }

    void FixedUpdate()
    {
        HandleGroundCheck();
        ApplyMovement();
        ApplyTurning();
        ApplyGravity();
        HandleJump();
    }

    private void SetRigidbody()
    {
        if (!Rigidbody)
        {
            Rigidbody = gameObject.AddComponent<Rigidbody>();
        }

        Rigidbody.mass = 70.0f;
        Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    private void CreateGroundCheck()
    {
        GroundCheck = new GameObject("GroundCheck").transform;
        GroundCheck.parent = transform;
        GroundCheck.localPosition = new Vector3(0, -0.9f, 0);
    }

    private void HandleGroundCheck()
    {
        IsGrounded = Physics.CheckSphere(GroundCheck.position, GroundCheckDistance, GroundMask);
    }

    private void ApplyMovement()
    {
        Vector3 desiredVelocity = (transform.forward * MoveInput + transform.right * StrafeInput) * MaxMoveSpeed;
        Vector3 velocityChange = (desiredVelocity - new Vector3(Rigidbody.velocity.x, 0, Rigidbody.velocity.z));

        velocityChange = Vector3.ClampMagnitude(velocityChange, Acceleration * Time.fixedDeltaTime);
        Rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    private void ApplyTurning()
    {
        if (Mathf.Abs(TurnInput) > 0.01f)
        {
            float turnAmount = TurnInput * TurnSpeed * Time.fixedDeltaTime;
            Rigidbody.MoveRotation(Rigidbody.rotation * Quaternion.Euler(0, turnAmount, 0));
        }
    }

    private void ApplyGravity()
    {
        if (!IsGrounded)
        {
            Rigidbody.AddForce(Vector3.down * Gravity, ForceMode.Acceleration);
        }
    }

    private void HandleJump()
    {
        if (JumpInput)
        {
            Rigidbody.velocity = new Vector3(Rigidbody.velocity.x, JumpForce, Rigidbody.velocity.z);
            JumpInput = false;
        }
    }

    public void SetMoveInput(float input)
    {
        MoveInput = Mathf.Clamp(input, -1.0f, 1.0f);
    }

    public void SetStrafeInput(float input)
    {
        StrafeInput = Mathf.Clamp(input, -1.0f, 1.0f);
    }

    public void SetTurnInput(float input)
    {
        TurnInput = Mathf.Clamp(input, -1.0f, 1.0f);
    }

    public void SetJumpInput(bool input)
    {
        JumpInput = input;
    }
}
