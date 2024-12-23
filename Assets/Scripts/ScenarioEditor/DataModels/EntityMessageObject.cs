using Newtonsoft.Json;

[System.Serializable]
public class EntityMessageObject
{
    [JsonProperty("id")]
    public string ID { get; set; }

    [JsonProperty("latitude")]
    public float Latitude { get; set; }

    [JsonProperty("longitude")]
    public float Longitude { get; set; }

    [JsonProperty("altitude")]
    public float Altitude { get; set; }

    [JsonProperty("pitch")]
    public float Pitch { get; set; }

    [JsonProperty("heading")]
    public float Heading { get; set; }

    [JsonProperty("roll")]
    public float Roll { get; set; }

    [JsonProperty("affiliation")]
    public int Affiliation { get; set; }

    [JsonProperty("attachedWaypoints")]
    public string[] AttachedWaypoints { get; set; }

    [JsonProperty("fuel")]
    public float Fuel { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }
}
