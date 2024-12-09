using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using StatePredicators;

public class GenericInfantryController : MonoBehaviour
{
    [Header("Go To Tasks")]
    public List<Vector3> GoToTasks = new();

    [Header("Physics Model")]
    public GenericInfantryPhysicsModel PhysicsModel;

    public delegate void StateHandler(GenericInfantryController controller);

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
            PhysicsModel.SetMoveInput(EvaluateMoveInput(currentTask));
            PhysicsModel.SetTurnInput(EvaluateTurnInput(currentTask));
        }
    }

    private float EvaluateMoveInput(Vector3 target)
    {
        float distance = Vector3.Distance(transform.position, target);
        return Mathf.Clamp(distance * 0.1f, 0.0f, 1.0f);
    }

    private float EvaluateTurnInput(Vector3 target)
    {
        Vector3 directionToTarget = (target - transform.position).normalized;
        float angleToTarget = Vector3.SignedAngle(transform.forward, directionToTarget, Vector3.up);
        return Mathf.Clamp(angleToTarget * 0.01f, -1.0f, 1.0f);
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
        PhysicsModel.SetMoveInput(0.0f);
        PhysicsModel.SetTurnInput(0.0f);
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
            PhysicsModel = gameObject.GetComponent<GenericInfantryPhysicsModel>();
        }
    }
}
