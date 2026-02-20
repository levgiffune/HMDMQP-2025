using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class WaypointManager : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject waypointPrefab;

    [Header("Compass")]
    public CompassManager compass;

    [Header("Demo Assets")]
    public GameObject demoPreviewPrefab;

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
        Vector3 hb = playerCamera.position + playerCamera.forward * 1.5f;
        GenerateDemoWaypoint(hb, "Home Base", "Network access point. All waypoint widgets are active on this marker: description panel, media display, and preview orb.");
        
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

    private void GenerateDemoWaypoint(Vector3 sp, string n, string d)
    {
        Waypoint defaultWaypoint = new Waypoint(sp, n);
        defaultWaypoint.desc = d;
        defaultWaypoint.color = Color.green;
        defaultWaypoint.iconType = WaypointIconType.POI;

        // Images for info card carousel
        Texture2D[] demoImages = Resources.LoadAll<Texture2D>("DemoMedia");
        if (demoImages != null && demoImages.Length > 0)
            defaultWaypoint.images = demoImages;

        // Video for WaypointMediaDisplay
        VideoClip demoVideo = Resources.Load<VideoClip>("DemoMedia/boston_atlas");
        if (demoVideo != null)
            defaultWaypoint.videoClip = demoVideo;

        // Preview prefab for WaypointPreviewOrb
        if (demoPreviewPrefab != null)
            defaultWaypoint.previewPrefab = demoPreviewPrefab;

        waypoints.Add(defaultWaypoint);
        CreateVisual(defaultWaypoint);

        if (WaypointMenuController.Instance != null)
        {
            WaypointMenuController.Instance.AddWaypointToList(defaultWaypoint);
        }
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
        }
        RemoveVisual(wpid);
        return toRemove != null;
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

    public void RemoveVisual(string waypointId)
    {
        if (string.IsNullOrEmpty(waypointId)) return;

        for (int i = 0; i < activeVisuals.Count; i++)
        {
            WaypointVisual visual = activeVisuals[i];
            if (visual == null)
            {
                continue;
            }

            Waypoint data = visual.GetWaypointData();
            if (data != null && data.id == waypointId)
            {
                activeVisuals.RemoveAt(i);
                Destroy(visual.gameObject);
                return;
            }
        }
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
