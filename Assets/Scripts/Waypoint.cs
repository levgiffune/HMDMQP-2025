using System;
using UnityEngine;


[System.Serializable]
public class Waypoint
{
    public string id; 
    public string name;
    public string desc;

    public Vector3 position;
    public Quaternion rotation;

    public Color color;
    public WaypointIconType iconType;

    public bool isActive;

    public Waypoint(Vector3 pos, string wpname = "New Waypoint")
    {
        id = Guid.NewGuid().ToString();
        position = pos;
        name = wpname;
        desc = "";
        color = Color.cyan;
        iconType = WaypointIconType.Standard;
        isActive = true;
    }

    public float DistanceFrom(Vector3 otherPosition)
    {
        return Vector3.Distance(position, otherPosition);
    }

    public void UpdatePosition(Vector3 newPosition)
    {
        position = newPosition;
    }
}

[System.Serializable]
public enum WaypointIconType
{
    Standard, 
    POI, 
    Warning
}

