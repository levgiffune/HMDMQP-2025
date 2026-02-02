using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class WaypointListBuilder:MonoBehaviour{
    public List<Waypoint> WaypointList;


    void Start(){
        Debug.Log($"WaypointListBuilder Start - WaypointManager.Instance: {WaypointManager.Instance}");
        Debug.Log($"WaypointListBuilder Start - MinimapController.Instance: {MinimapController.Instance}");
        Debug.Log($"WaypointList count: {WaypointList.Count}");
        foreach(Waypoint w in WaypointList){
            WaypointManager.Instance.CreateWaypoint(w);
            WaypointMenuController.Instance.AddWaypointToListPublic(w);
        }
    }
}