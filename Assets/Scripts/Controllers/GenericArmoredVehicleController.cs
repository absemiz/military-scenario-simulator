using Cysharp.Threading.Tasks.CompilerServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GenericArmoredVehicleController : MonoBehaviour
{
    [Header("Go To Tasks")]
    public List<Vector2> GoToTasks = new();

    [Header("Physics Model")]
    public GenericArmoredVehiclePhysicsModel PhysicsModel;

    void Start()
    {
        InitializePhysicsModel();
    }

    void FixedUpdate()
    {
        HandleGoToTasks();
    }

    private void HandleGoToTasks()
    {
        if (GoToTasks.Count == 0)
        {
            return;
        }

        Vector2 currentTask = GoToTasks.Last();

        if (GoToTaskCompleted(currentTask))
        {
            GoToTasks.Remove(currentTask);
            Debug.Log("Task completed.");
        }
        else
        {
            PhysicsModel.SetSteering(EvaluateSteering(currentTask));
            PhysicsModel.SetThrust(EvaluateThrust(currentTask));
        }
    }

    private float EvaluateThrust(Vector2 target)
    {
        Vector2 currentPosition = new Vector2(transform.position.z, transform.position.x);
        Vector2 pathVector = target - currentPosition;

        return Mathf.Clamp(pathVector.magnitude, 0.0f, 1.0f);
    }

    private float EvaluateSteering(Vector2 target)
    {
        Vector2 currentPosition = new Vector2(transform.position.z, transform.position.x);
        Vector2 forwardDirection = new Vector2(transform.forward.z, transform.forward.x);

        Vector2 targetDirection = (target - currentPosition).normalized;
       

        float angle = Vector2.SignedAngle(forwardDirection, targetDirection);

        return Mathf.Clamp(angle, -1.0f, 1.0f);
    }

    public void AddGoToTask(Vector2 targetPosition)
    {
        GoToTasks.Add(targetPosition);
    }

    public void AddGoToTask(float x, float z)
    {
        AddGoToTask(new Vector2(x, z));
    }

    private bool GoToTaskCompleted(float z, float x)
    {
        float epsilon = 1.0F;

        return MathF.Abs(x - transform.position.x) < epsilon && MathF.Abs(z - transform.position.z) < epsilon;
    }

    private bool GoToTaskCompleted(Vector2 targetPosition)
    {
        return GoToTaskCompleted(targetPosition.x, targetPosition.y);
    }

    private void InitializePhysicsModel()
    {
        if (!PhysicsModel)
        {
            PhysicsModel = gameObject.GetComponent<GenericArmoredVehiclePhysicsModel>();
        }
    }
}
