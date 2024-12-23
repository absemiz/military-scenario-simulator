using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializationMessage
{
    public List<EntityMessageObject> Entities { get; set; }
    public List<Path> Paths { get; set; }
    public List<Waypoint> Waypoints { get; set; }
}
