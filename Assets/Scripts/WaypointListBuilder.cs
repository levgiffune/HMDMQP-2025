using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class WaypointListBuilder:MonoBehaviour{
    public List<Waypoint> Waypoints;
    public TextAsset jsonFile;

    void Start(){
        Debug.Log($"WaypointListBuilder Start - WaypointManager.Instance: {WaypointManager.Instance}");
        Debug.Log($"WaypointListBuilder Start - MinimapController.Instance: {MinimapController.Instance}");

        WaypointList jsonIn = JsonUtility.FromJson<WaypointList>(jsonFile.text);

        foreach (Waypoint wp in jsonIn.Waypoints)
        {
            Debug.Log("Found waypoint: " + wp.name);
        }

        Waypoints.AddRange(jsonIn.Waypoints);

        Debug.Log($"WaypointList count: {Waypoints.Count}");

        foreach(Waypoint w in Waypoints){
            WaypointManager.Instance.CreateWaypoint(w);
            WaypointMenuController.Instance.AddWaypointToList(w);
        }
    }
}