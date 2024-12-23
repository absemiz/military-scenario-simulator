using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UpdateMessageObject
{
    public List<EntityMessageObject> entities { get; set; } = new();
}
