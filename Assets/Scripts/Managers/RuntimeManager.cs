using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;

using UnityEngine;


using Newtonsoft.Json;

public class RuntimeManager : MonoBehaviour
{
    public static RuntimeManager Instance { get; private set; }

    private readonly Dictionary<string, EntityMessageObject> EntityCache = new();
    private readonly List<GameObject> TrackedEntities = new();
    private readonly object CacheLock = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    private void Start()
    {
    }

    private void Update()
    {
        lock (CacheLock)
        {
            EntityCache.Clear();

            foreach (GameObject entity in TrackedEntities)
            {
                LatitudeLongitute latitudeLongitudePosition = LatitudeLongitute.FromUnityCoordinates(entity.transform.position.z, entity.transform.position.x);

                EntityMessageObject entityMessageObject = new()
                {
                    ID = entity.name,
                    Latitude = latitudeLongitudePosition.Latitude,
                    Longitude = latitudeLongitudePosition.Longitude,
                    Altitude = entity.transform.position.y,
                    Pitch = entity.transform.rotation.eulerAngles.x,
                    Heading = entity.transform.rotation.eulerAngles.y,
                    Roll = entity.transform.rotation.eulerAngles.z,
                    Affiliation = 0,
                    AttachedWaypoints = new string[0],
                    Fuel = 100,
                    Type = "None"
                };

                EntityCache[entity.name] = entityMessageObject;
            }
        }
    }

    public void AddTrackedEntity(GameObject entityGameObject)
    {
        if (!TrackedEntities.Contains(entityGameObject))
        {
            TrackedEntities.Add(entityGameObject);
        }
    }

    public string GetUpdateResponse()
    {
        lock (CacheLock)
        {
            UpdateMessageObject updateMessage = new()
            {
                entities = new List<EntityMessageObject>(EntityCache.Values)
            };

            string asJSON = JsonConvert.SerializeObject(updateMessage, Formatting.Indented);

            return asJSON;
        }
    }

}