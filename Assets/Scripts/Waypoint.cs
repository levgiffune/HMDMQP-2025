using System;
using UnityEngine;
using UnityEngine.Video;

[System.Serializable]
public class Waypoint
{
    public string id; 
    public string name;
    public string desc;

    public Vector3 position;
    public Quaternion rotation;

    public Color color;

    // Tour mode sequencing (-1 = not in tour)
    public int tourOrder = -1;

    // Model reference (loads from Resources by name)
    public string model;
    public string modelName;
    public string modelDesc;

    // Media / content references (optional)
    public string[] imageNames;
    [System.NonSerialized] public Texture2D[] images;
    public VideoClip videoClip;

    public Waypoint(
        Color c,
        Vector3 p,
        string n = "New Waypoint", 
        string d = null)
    {
        id = Guid.NewGuid().ToString();
        position = p;
        name = n;
        desc = d;
        color = c;
    }

    public Waypoint(
        Vector3 p, 
        string n = "New Waypoint", 
        string d = null)
    {
        id = Guid.NewGuid().ToString();
        position = p;
        name = n;
        desc = d;
        color = Color.cyan;
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
