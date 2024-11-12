using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LatitudeLongitute
{
    private const float metersPerDegree = 111E3f;

    LatitudeLongitute(float latitude, float longitude)
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

    public static LatitudeLongitute FromUnityCoordinates(float X, float Y)
    {
        return new LatitudeLongitute(Y / metersPerDegree, X / metersPerDegree);
    }
}
