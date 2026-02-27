using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject waypointPrefab;

    [Header("Compass")]
    public CompassManager compass;

    // Singleton
    public static WaypointManager Instance { get; private set; }

    // Reference to player camera for waypoint orientation
    public Transform playerCamera;

    private List<Waypoint> waypoints = new List<Waypoint>();
    private List<WaypointVisual> activeVisuals = new List<WaypointVisual>();

    // Getter for waypoints list
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

    /// <summary>
    /// Register a predefined waypoint and create its visual.
    /// Called by WaypointListBuilder on startup.
    /// </summary>
    public Waypoint CreateWaypoint(Waypoint w)
    {
        waypoints.Add(w);
        CreateVisual(w);
        return w;
    }

    private void CreateVisual(Waypoint waypoint)
    {
        if (waypointPrefab == null) return;

        GameObject visualObj = Instantiate(waypointPrefab);
        WaypointVisual visual = visualObj.GetComponent<WaypointVisual>();
        visual.Initialize(waypoint, playerCamera);
        activeVisuals.Add(visual);
    }

    public Waypoint GetWaypoint(string wpid)
    {
        return waypoints.Find(w => w.id == wpid);
    }

    public GameObject GetWaypointVisual(string waypointId)
    {
        WaypointVisual visual = activeVisuals.Find(v => v.GetWaypointData().id == waypointId);
        return visual != null ? visual.gameObject : null;
    }

    /// <summary>
    /// Returns waypoints that have a tourOrder >= 0, sorted by tourOrder.
    /// </summary>
    public List<Waypoint> GetWaypointsByTourOrder()
    {
        return waypoints
            .Where(w => w.tourOrder >= 0)
            .OrderBy(w => w.tourOrder)
            .ToList();
    }

    /// <summary>
    /// Get the WaypointVisual component for a given waypoint ID.
    /// </summary>
    public WaypointVisual GetVisualComponent(string waypointId)
    {
        return activeVisuals.Find(v => v.GetWaypointData().id == waypointId);
    }

    public void SaveWaypoints(List<Waypoint> waypointsToSave)
    {
        WaypointList list = new WaypointList { Waypoints = waypointsToSave };
        string json = JsonUtility.ToJson(list, true);
        string path = Path.Combine(Application.dataPath, "Resources/Waypoints.json");
        File.WriteAllText(path, json);
        Debug.Log($"WaypointManager: Saved {waypointsToSave.Count} waypoints to {path}");
    }
}
