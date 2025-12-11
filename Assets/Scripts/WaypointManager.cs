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
    private string highlightedWaypointId = null; 

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

    public Waypoint CreateWaypoint(Waypoint w){
        waypoints.Add(w);
        CreateVisual(w);

        return w;
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

    public GameObject GetWaypointVisual(string waypointId)
    {
        WaypointVisual visual = activeVisuals.Find(v => v.GetWaypointData().id == waypointId);
        return visual != null ? visual.gameObject : null;
    }

    public void HighlightWaypoint(string waypointId)
    {
        if (highlightedWaypointId != null)
        {
            WaypointVisual prevVisual = activeVisuals.Find(v => v.GetWaypointData().id == highlightedWaypointId);
            if (prevVisual != null)
            {
                prevVisual.SetHighlighted(false);
            }
        }
        
        WaypointVisual visual = activeVisuals.Find(v => v.GetWaypointData().id == waypointId);
        if (visual != null)
        {
            visual.SetHighlighted(true);
            highlightedWaypointId = waypointId;
        }
    }
}
