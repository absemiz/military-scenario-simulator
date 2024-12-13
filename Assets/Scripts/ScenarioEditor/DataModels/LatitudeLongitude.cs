using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LatitudeLongitute
{

    private const float metersPerDegree = 111E3F;

    public LatitudeLongitute(float latitude, float longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }
    public float Latitude { get; set; }
    public float Longitude { get; set; }

    public Vector2 ToUnityCoordinates()
    {
        return new Vector2(Longitude * metersPerDegree, Latitude * metersPerDegree);
    }

    public void ToUnityCoordinates(out float z, out float x)
    {
        z = Latitude * metersPerDegree;
        x = Longitude * metersPerDegree;
    }

    public static LatitudeLongitute FromUnityCoordinates(float z, float x)
    {
        return new LatitudeLongitute(z / metersPerDegree, x / metersPerDegree);
    }
}
