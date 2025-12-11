using System;
using UnityEngine;
using TMPro;


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


    public Waypoint(
        Color c,
        Vector3 p,
        string n = "New Waypoint", 
        string d = "",
        WaypointIconType i = WaypointIconType.Standard){
        id = Guid.NewGuid().ToString();
        position = p;
        name = n;
        desc = d;
        color = c;
        iconType = i;
    }

    public Waypoint(
        Vector3 p, 
        string n = "New Waypoint", 
        string d = "", 
        WaypointIconType i = WaypointIconType.Standard){
        id = Guid.NewGuid().ToString();
        position = p;
        name = n;
        desc = d;
        color = Color.cyan;
        iconType = i;
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