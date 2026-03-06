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
    private bool proximityEnabled = false;

    public bool IsPlayerInRange => isPlayerInRange;

    public void Initialize(Waypoint waypoint, Transform playerCamera)
    {
        waypointData = waypoint;
        cameraTransform = playerCamera;
    }

    /// <summary>
    /// Enable proximity detection for this waypoint.
    /// If the player is already inside the radius, triggers content immediately.
    /// </summary>
    public void Activate()
    {
        proximityEnabled = true;
        if (isPlayerInRange)
        {
            waypointVisual?.SetProximityActive(true);
            OnPlayerEntered?.Invoke();
        }
    }

    /// <summary>
    /// Disable proximity detection. Hides content if the player is currently in range.
    /// </summary>
    public void Deactivate()
    {
        proximityEnabled = false;
        if (isPlayerInRange)
        {
            waypointVisual?.SetProximityActive(false);
        }
    }

    private void Update()
    {
        if (cameraTransform == null || waypointData == null) return;

        float distance = waypointData.DistanceFrom(cameraTransform.position);
        bool inRange = distance <= activationDistance;

        if (inRange && !isPlayerInRange)
        {
            isPlayerInRange = true;
            if (proximityEnabled && waypointVisual != null)
            {
                waypointVisual.SetProximityActive(true);
                waypointVisual.descriptionCard.StartTTS();
            }
            OnPlayerEntered?.Invoke();
        }
        else if (!inRange && isPlayerInRange)
        {
            isPlayerInRange = false;
            if (proximityEnabled && waypointVisual != null)
            {
                waypointVisual.SetProximityActive(false);
                waypointVisual.descriptionCard.StopTTS();
            }
            OnPlayerExited?.Invoke();
        }
    }

    public string GetWaypointId()
    {
        return waypointData?.id;
    }
}
