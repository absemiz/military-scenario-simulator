using Newtonsoft.Json.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializationManager : MonoBehaviour
{
    [SerializeField]
    private ScenarioEditorServer Server;

    [SerializeField]
    private GameObject ArmoredVehiclePrefab;

    [SerializeField]
    private GameObject FixedWingPrefab;

    [SerializeField]
    private GameObject RotaryWingPrefab;

    [SerializeField]
    private GameObject InfantryPrefab;

    private bool Initialized = false;

    private delegate void CreateFunction(EntityMessageObject entity, List<Waypoint> waypoints);

    void Start()
    {
        InitializeServer();
        InitalizePrefabs();
    }

    void Update()
    {
        if (!Initialized && ScenarioInitialized())
        {
            InitializeScenario();
            Initialized = true;
        }
    }

    private void InitializeServer()
    {
        if (Server == null)
        {
            Debug.LogError("Server reference must be set.");
        }
    }

    private void InitalizePrefabs()
    {
        bool notNullPrefabs = ArmoredVehiclePrefab && FixedWingPrefab && RotaryWingPrefab && InfantryPrefab;
        if (!notNullPrefabs)
        {
            Debug.LogError("All prefabs must be set.");
        }
    }

    private bool ScenarioInitialized()
    {
        return Server.GetInitializationMessage() != null;
    }

    private void InitializeScenario()
    {
        InitializeEntities();
    }

    private void InitializeEntities()
    {
        Dictionary<string, CreateFunction> EntityCreationHandlerTable = new Dictionary<string, CreateFunction>
        {
            { "ArmoredVehicle", CreateArmoredVehicle },
            { "FixedWing", CreateFixedWing },
            { "RotaryWing", CreateRotaryWing },
            { "Infantry", CreateInfantry }
        };

        foreach (EntityMessageObject entity in Server.GetInitializationMessage().Entities)
        {
            if (EntityCreationHandlerTable.ContainsKey(entity.Type))
            {
                EntityCreationHandlerTable[entity.Type](entity, Server.GetInitializationMessage().Waypoints);
            }
            else
            {
                Debug.LogWarning($"Unknown entity type: {entity.Type}");
            }
        }
    }

    private void CreateArmoredVehicle(EntityMessageObject entityMessage, List<Waypoint> waypoints)
    {
        GameObject armoredVehicle = InstantiateEntity(ArmoredVehiclePrefab, entityMessage);

        GenericArmoredVehicleController armoredVehicleController = armoredVehicle.GetComponent<GenericArmoredVehicleController>();

        AttachTasks(armoredVehicle, entityMessage, waypoints, armoredVehicleController);

        RuntimeManager.Instance.AddTrackedEntity(armoredVehicle);
    }

    private void CreateFixedWing(EntityMessageObject entityMessageObject, List<Waypoint> waypoints)
    {
        InstantiateEntity(FixedWingPrefab, entityMessageObject);
    }

    private void CreateRotaryWing(EntityMessageObject entityMessageObject, List<Waypoint> waypoints)
    {
        InstantiateEntity(RotaryWingPrefab, entityMessageObject);
    }

    private void CreateInfantry(EntityMessageObject entityMessageObject, List<Waypoint> waypoints)
    {
        InstantiateEntity(InfantryPrefab, entityMessageObject);
    }

    private GameObject InstantiateEntity(GameObject prefab, EntityMessageObject entityMessageObject)
    {
        LatitudeLongitute position = new LatitudeLongitute(Convert.ToSingle(entityMessageObject.Latitude), Convert.ToSingle(entityMessageObject.Longitude));

        position.ToUnityCoordinates(out float z, out float x);

        Vector3 worldPosition = new Vector3(x, Convert.ToSingle(entityMessageObject.Altitude), z);

        Quaternion rotation = Quaternion.Euler(Convert.ToSingle(entityMessageObject.Pitch), Convert.ToSingle(entityMessageObject.Heading), Convert.ToSingle(entityMessageObject.Roll));

        GameObject entityGameObject = Instantiate(prefab, worldPosition, rotation);

        entityGameObject.tag = "MilitaryEntity";
        entityGameObject.name = $"{entityMessageObject.ID}";

        return entityGameObject;
    }

    private void AttachTasks(GameObject entity, EntityMessageObject entityMessage, List<Waypoint> waypoints, IEntityController controller)
    {
        foreach (string attachedWaypointID in entityMessage.AttachedWaypoints)
        {
            Waypoint targetWaypoint = waypoints.Find(waypoint => waypoint.ID.Equals(attachedWaypointID));

            if (targetWaypoint != null)
            {
                LatitudeLongitute latitudeLongitute = new(Convert.ToSingle(targetWaypoint.Position[0]), Convert.ToSingle(targetWaypoint.Position[1]));

                latitudeLongitute.ToUnityCoordinates(out float z, out float x);

                controller.AddGoToTask(x, 0, z);
            }
        }
    }
}
