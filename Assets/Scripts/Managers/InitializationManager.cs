using Newtonsoft.Json.Bson;
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

    private delegate void CreateFunction(MilitaryEntity entity);

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

        foreach (MilitaryEntity entity in Server.GetInitializationMessage().Entities)
        {
            if (EntityCreationHandlerTable.ContainsKey(entity.Type))
            {
                EntityCreationHandlerTable[entity.Type](entity);
            }
            else
            {
                Debug.LogWarning($"Unknown entity type: {entity.Type}");
            }
        }
    }

    private void CreateArmoredVehicle(MilitaryEntity entity)
    {
        InstantiateEntity(ArmoredVehiclePrefab, entity);
    }

    private void CreateFixedWing(MilitaryEntity entity)
    {
        InstantiateEntity(FixedWingPrefab, entity);
    }

    private void CreateRotaryWing(MilitaryEntity entity)
    {
        InstantiateEntity(RotaryWingPrefab, entity);
    }

    private void CreateInfantry(MilitaryEntity entity)
    {
        InstantiateEntity(InfantryPrefab, entity);
    }

    private void InstantiateEntity(GameObject prefab, MilitaryEntity entity)
    {
        Vector2 position = new LatitudeLongitute((float)entity.Latitude, (float)entity.Longitude).ToUnityCoordinates();

        Vector3 worldPosition = new Vector3(position.x, (float)entity.Altitude, position.y);

        Quaternion rotation = Quaternion.Euler((float)entity.Pitch, (float)entity.Heading, (float)entity.Roll);

        GameObject entityGameObject = Instantiate(prefab, worldPosition, rotation);

        entityGameObject.name = $"{entity.Type}_{entity.ID}";
    }
}
