using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class WaypointListBuilder:MonoBehaviour{
    public List<Waypoint> WaypointList;


    void Start(){
        foreach(Waypoint w in WaypointList){
            WaypointManager.Instance.CreateWaypoint(w);
        }
    }
}