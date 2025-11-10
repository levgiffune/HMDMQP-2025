using System.Collections.Generic;
using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject waypointPrefab;
    public Transform playerCamera;

    [Header("Compass")]
    public CompassManager compass;
    // ensure single instance of waypoint manager
    public static WaypointManager Instance {get; private set;}

    // waypoint data representation
    private List<Waypoint> waypoints = new List<Waypoint>(); 
    // waypoint visual representation
    private List<WaypointVisual> activeVisuals = new List<WaypointVisual>(); 

    // getter for waypoints list
    public List<Waypoint> Waypoints => waypoints;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Waypoint CreateWaypoint(Vector3 position, string name = "New Waypoint")
    {
        Waypoint newWaypoint = new Waypoint(position, name);
        waypoints.Add(newWaypoint);
        CreateVisual(newWaypoint);



        return newWaypoint;
    }

    private void CreateVisual(Waypoint waypoint)
    {
        if (waypointPrefab == null) return;

        GameObject visualObj = Instantiate(waypointPrefab);
        compass.Waypoint = visualObj;
        WaypointVisual visual = visualObj.GetComponent<WaypointVisual>();
        visual.Initialize(waypoint, playerCamera);
        activeVisuals.Add(visual);
    }

    public bool DeleteWaypoint(string wpid)
    {
        Waypoint toRemove = waypoints.Find(w => w.id == wpid);
        if (toRemove != null)
        {
            waypoints.Remove(toRemove);
            return true;
        }
        return false;
    }

    public Waypoint GetWaypoint(string wpid)
    {
        return waypoints.Find(w => w.id == wpid);
    }

    public void ClearAllWaypoints()
    {
        waypoints.Clear();
    }
}
