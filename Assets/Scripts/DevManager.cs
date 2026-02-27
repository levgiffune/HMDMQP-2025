using UnityEngine;
using UnityEngine.InputSystem;

public class DevManager : MonoBehaviour
{
    [Header("Dev Mode Settings")]
    public bool devModeEnabled = false;
    public InputActionProperty devToggleAction;

    public Transform playerCamera;

    void Start()
    {
        if (devModeEnabled)
        {
            devToggleAction.action.Enable();
            devToggleAction.action.performed += _ => SpawnWaypoint();
        }
    }
    [ContextMenu("Spawn Waypoint")]
    void SpawnWaypoint()
    {
        Vector3 pos = playerCamera != null ? playerCamera.position : Vector3.zero;
        WaypointManager.Instance.CreateWaypoint(new Waypoint(pos));
        Debug.Log($"Dev waypoint created at {pos}");
        Debug.Log(WaypointManager.Instance);
        Debug.Log( WaypointListBuilder.Instance);
        WaypointListBuilder.Instance.SaveWaypoints(WaypointManager.Instance.Waypoints);
    }
}
