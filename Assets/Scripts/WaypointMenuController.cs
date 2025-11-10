using UnityEngine;
using OVR; // Add this

public class WaypointMenuController : MonoBehaviour
{
    public Transform playerTransform; // CenterEyeAnchor

    [Header("UI References")]
    public Transform waypointListContainer;
    public GameObject waypointListItemPrefab;
    
    void Update()
    {
        // Check if A button pressed on right controller
        if (OVRInput.GetDown(OVRInput.Button.Two)) 
        {
            CreateWaypoint();
        }
    }
    
    void CreateWaypoint()
    {
        if (playerTransform == null)
        {
            Debug.LogError("Player transform not assigned!");
            return;
        }
        
        Vector3 spawnPosition = playerTransform.position;
        Waypoint newWaypoint = WaypointManager.Instance.CreateWaypoint( 
        spawnPosition, 
        $"Waypoint {WaypointManager.Instance.Waypoints.Count + 1}"
        );

        AddWaypointToList(newWaypoint);
    }

    void AddWaypointToList(Waypoint waypoint)
    {
        if (waypointListItemPrefab == null || waypointListContainer == null) return;
        
        GameObject itemObj = Instantiate(waypointListItemPrefab, waypointListContainer);
        WaypointListItem item = itemObj.GetComponent<WaypointListItem>();
        item.Setup(waypoint);
    }
}