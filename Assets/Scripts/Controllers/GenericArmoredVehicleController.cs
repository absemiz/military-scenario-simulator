using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using StatePredicators;

public class GenericArmoredVehicleController : MonoBehaviour
{
    [Header("Go To Tasks")]
    public List<Vector3> GoToTasks = new();

    [Header("Physics Model")]
    public GenericArmoredVehiclePhysicsModel PhysicsModel;

    public delegate void StateHandler(GenericArmoredVehicleController controller);

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
            PhysicsModel.SetSteering(EvaluateSteering(currentTask));
            PhysicsModel.SetThrust(EvaluateThrust(currentTask));
        }
    }

    private float EvaluateThrust(Vector3 target)
    {
        Vector3 currentPosition = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 pathVector = new Vector3(target.x, 0, target.z) - currentPosition;

        return Mathf.Clamp(pathVector.magnitude, 0.0f, 1.0f);
    }

    private float EvaluateSteering(Vector3 target)
    {
        Vector3 currentPosition = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 forwardDirection = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
        Vector3 targetDirection = (new Vector3(target.x, 0, target.z) - currentPosition).normalized;

        float angle = Vector3.SignedAngle(forwardDirection, targetDirection, Vector3.up);

        return Mathf.Clamp(angle / 45.0f, -1.0f, 1.0f);
    }

    public void AddGoToTask(Vector3 targetPosition)
    {
        Debug.Log($"Go To Task Added: {targetPosition}");

        GoToTasks.Add(targetPosition);
    }

    public void AddGoToTask(float x, float y, float z)
    {
        AddGoToTask(new Vector3(x, y, z));
    }

    public void Stop()
    {
        if (PhysicsModel.Rigidbody.velocity.magnitude > 0.0f)
        {
            PhysicsModel.SetBrakeState(true);
        }
        else
        {
            PhysicsModel.SetBrakeState(false);
        }
    }

    void AddRigidbodyStateHandler(RigidbodyPredicator stateFilter, StateHandler stateHandler)
    {
        RigidbodyStateHandlers.Add(stateFilter, stateHandler);
    }

    private bool GoToTaskCompleted(Vector3 targetPosition)
    {
        float epsilon = 10.0f;

        return Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(targetPosition.x, 0, targetPosition.z)) < epsilon;
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

    private void InitializePhysicsModel()
    {
        if (!PhysicsModel)
        {
            PhysicsModel = gameObject.GetComponent<GenericArmoredVehiclePhysicsModel>();
        }
    }
}
