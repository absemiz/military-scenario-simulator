using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using StatePredicators;

public class GenericRotaryWingController : MonoBehaviour
{
    [Header("Go To Tasks")]
    public List<Vector3> GoToTasks = new();

    [Header("Physics Model")]
    public GenericRotaryWingPhysicsModel PhysicsModel;

    public delegate void StateHandler(GenericRotaryWingController controller);

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
            PhysicsModel.SetCollectiveInput(EvaluateCollective(currentTask));
            PhysicsModel.SetCyclicPitchInput(EvaluateCyclicPitch(currentTask));
            PhysicsModel.SetCyclicRollInput(EvaluateCyclicRoll(currentTask));
        }
    }

    private float EvaluateCollective(Vector3 target)
    {
        float heightDifference = target.y - transform.position.y;

        return Mathf.Clamp(heightDifference * 0.1f, -1.0f, 1.0f);
    }

    private float EvaluateCyclicPitch(Vector3 target)
    {
        Vector3 currentPosition = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 targetPosition = new Vector3(target.x, 0, target.z);
        Vector3 directionToTarget = (targetPosition - currentPosition).normalized;

        float forwardAngle = Vector3.Dot(transform.forward, directionToTarget);

        return Mathf.Clamp(forwardAngle, -1.0f, 1.0f);
    }

    private float EvaluateCyclicRoll(Vector3 target)
    {
        Vector3 currentPosition = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 targetPosition = new Vector3(target.x, 0, target.z);
        Vector3 directionToTarget = (targetPosition - currentPosition).normalized;

        float sideAngle = Vector3.Dot(transform.right, directionToTarget);

        return Mathf.Clamp(sideAngle, -1.0f, 1.0f);
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
        PhysicsModel.SetCollectiveInput(0.0f);
        PhysicsModel.SetCyclicPitchInput(0.0f);
        PhysicsModel.SetCyclicRollInput(0.0f);
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
        float epsilon = 1.0f;

        return Vector3.Distance(transform.position, targetPosition) < epsilon;
    }

    private void InitializePhysicsModel()
    {
        if (!PhysicsModel)
        {
            PhysicsModel = gameObject.GetComponent<GenericRotaryWingPhysicsModel>();
        }
    }
}
