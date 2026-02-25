using System;
using UnityEngine;

/// <summary>
/// Detects when the player enters/exits a proximity radius around a waypoint.
/// Shows/hides the waypoint's widgets (3D model, InfoCard, DescriptionCard).
/// </summary>
public class ProximityActivator : MonoBehaviour
{
    [Header("Settings")]
    public float activationDistance = 2f;

    [Header("References")]
    public WaypointVisual waypointVisual;

    // Events for other systems to subscribe (e.g., TourManager)
    public event Action OnPlayerEntered;
    public event Action OnPlayerExited;

    private Transform cameraTransform;
    private Waypoint waypointData;
    private bool isPlayerInRange = false;

    public bool IsPlayerInRange => isPlayerInRange;

    public void Initialize(Waypoint waypoint, Transform playerCamera)
    {
        waypointData = waypoint;
        cameraTransform = playerCamera;
    }

    private void Update()
    {
        if (cameraTransform == null || waypointData == null) return;

        float distance = waypointData.DistanceFrom(cameraTransform.position);
        bool inRange = distance <= activationDistance;

        if (inRange && !isPlayerInRange)
        {
            // Player entered range
            isPlayerInRange = true;
            if (waypointVisual != null)
            {
                waypointVisual.SetProximityActive(true);
            }
            OnPlayerEntered?.Invoke();
        }
        else if (!inRange && isPlayerInRange)
        {
            // Player exited range
            isPlayerInRange = false;
            if (waypointVisual != null)
            {
                waypointVisual.SetProximityActive(false);
            }
            OnPlayerExited?.Invoke();
        }
    }

    public string GetWaypointId()
    {
        return waypointData?.id;
    }
}
