using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LatitudeLongitute
{
    private static readonly float practicalScale = 1E-1F;
    private static readonly float metersPerDegree = 111*1E3F;
    private static readonly float effectiveMetersPerDegree = practicalScale * metersPerDegree;

    public LatitudeLongitute(float latitude, float longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }
    public float Latitude { get; set; }
    public float Longitude { get; set; }

    public Vector2 ToUnityCoordinates()
    {
        return new Vector2(Longitude * metersPerDegree, Latitude * effectiveMetersPerDegree);
    }

    public void ToUnityCoordinates(out float z, out float x)
    {
        z = Latitude * effectiveMetersPerDegree;
        x = Longitude * effectiveMetersPerDegree;
    }

    public static LatitudeLongitute FromUnityCoordinates(float z, float x)
    {
        return new LatitudeLongitute(z / effectiveMetersPerDegree, x / effectiveMetersPerDegree);
    }
}
