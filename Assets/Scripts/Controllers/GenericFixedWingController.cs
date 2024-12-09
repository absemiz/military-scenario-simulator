using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using StatePredicators;

public class GenericFixedWingController : MonoBehaviour
{
    [Header("Go To Tasks")]
    public List<Vector3> GoToTasks = new();

    [Header("Physics Model")]
    public GenericFixedWingPhysicsModel PhysicsModel;

    public delegate void StateHandler(GenericFixedWingController controller);

    private Dictionary<RigidbodyPredicator, StateHandler> RigidbodyStateHandlers = new();

    void Start()
    {
        InitializePhysicsModel();
    }

    void FixedUpdate()
    {
        HandleGoToTasks();
        ApplyRigidbodyStateHandlers();
    }

    private void HandleGoToTasks()
    {
        if (GoToTasks.Count == 0)
        {
            Stop();
            return;
        }

        Vector3 currentTask = GoToTasks.Last();

        if (GoToTaskCompleted(currentTask))
        {
            GoToTasks.Remove(currentTask);
        }
        else
        {
            PhysicsModel.SetThrustInput(EvaluateThrust(currentTask));
            PhysicsModel.SetPitchInput(EvaluatePitch(currentTask));
            PhysicsModel.SetRollInput(EvaluateRoll(currentTask));
            PhysicsModel.SetYawInput(EvaluateYaw(currentTask));
        }
    }

    private float EvaluateThrust(Vector3 target)
    {
        float distance = Vector3.Distance(transform.position, target);

        return Mathf.Clamp(distance * 0.05f, 0.0f, 1.0f);
    }

    private float EvaluatePitch(Vector3 target)
    {
        Vector3 directionToTarget = (target - transform.position).normalized;

        float pitchAngle = Vector3.Dot(transform.forward, directionToTarget);
        return Mathf.Clamp(pitchAngle, -1.0f, 1.0f);
    }

    private float EvaluateRoll(Vector3 target)
    {
        Vector3 directionToTarget = (target - transform.position).normalized;

        float rollAngle = Vector3.Dot(transform.right, directionToTarget);
        return Mathf.Clamp(rollAngle, -1.0f, 1.0f);
    }

    private float EvaluateYaw(Vector3 target)
    {
        Vector3 directionToTarget = (target - transform.position).normalized;

        float yawAngle = Vector3.SignedAngle(transform.forward, directionToTarget, Vector3.up);
        return Mathf.Clamp(yawAngle * 0.01f, -1.0f, 1.0f);
    }

    public void AddGoToTask(Vector3 targetPosition)
    {
        GoToTasks.Add(targetPosition);
    }

    public void AddGoToTask(float x, float y, float z)
    {
        AddGoToTask(new Vector3(x, y, z));
    }

    public void Stop()
    {
        PhysicsModel.SetThrustInput(0.0f);
        PhysicsModel.SetPitchInput(0.0f);
        PhysicsModel.SetRollInput(0.0f);
        PhysicsModel.SetYawInput(0.0f);
    }

    void AddRigidbodyStateHandler(RigidbodyPredicator stateFilter, StateHandler stateHandler)
    {
        RigidbodyStateHandlers.Add(stateFilter, stateHandler);
    }

    private void ApplyRigidbodyStateHandlers()
    {
        foreach (KeyValuePair<RigidbodyPredicator, StateHandler> entry in RigidbodyStateHandlers)
        {
            if (entry.Key(PhysicsModel.Rigidbody))
            {
                entry.Value(this);
            }
        }
    }

    private bool GoToTaskCompleted(Vector3 targetPosition)
    {
        float epsilon = 5.0f;

        return Vector3.Distance(transform.position, targetPosition) < epsilon;
    }

    private void InitializePhysicsModel()
    {
        if (!PhysicsModel)
        {
            PhysicsModel = gameObject.GetComponent<GenericFixedWingPhysicsModel>();
        }
    }
}
