using System.Collections.Generic;
using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject waypointPrefab;

    [Header("Compass")]
    public CompassManager compass;

    // ensure single instance of waypoint manager
    public static WaypointManager Instance {get; private set;}

    // reference to player camera for waypoint orientation
    public Transform playerCamera;

    private List<Waypoint> waypoints = new List<Waypoint>(); 
    private List<WaypointVisual> activeVisuals = new List<WaypointVisual>();

    // getter for waypoints list
    public List<Waypoint> Waypoints => waypoints;

    void Start()
    {
        // Start of default waypoint creation
        Vector3 spawnPosition = playerCamera.position + playerCamera.forward * 1.5f;
        Waypoint defaultWaypoint = new Waypoint(spawnPosition, "Home Base");
        defaultWaypoint.desc = "This is your starting location.";
        defaultWaypoint.color = Color.green;
        defaultWaypoint.iconType = WaypointIconType.POI;
        
        waypoints.Add(defaultWaypoint);
        CreateVisual(defaultWaypoint);
        
        
        if (WaypointMenuController.Instance != null)
        {
            WaypointMenuController.Instance.AddWaypointToListPublic(defaultWaypoint);
        }
        // End of default waypoint creation
    }

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

    public void UpdateWaypointVisual(string waypointId)
    {
        Waypoint waypoint = GetWaypoint(waypointId);
        if (waypoint == null) return;

        WaypointVisual visual = activeVisuals.Find(v => v.GetWaypointData().id == waypointId);
        if (visual != null)
        {
            visual.UpdateAppearance(waypoint);
        }
    }
}
