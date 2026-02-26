using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Drives the Tour Mode sequence. Tracks current waypoint index, advances on A press,
/// fires PopupCards at transitions, and handles end-of-tour.
/// </summary>
public class TourManager : MonoBehaviour
{
    public static TourManager Instance { get; private set; }

    [Header("References")]
    public PopupCard popupCard;
    public WaypointLineConnector lineConnector;
    public CompassManager compass;

    [Header("Settings")]
    public Color tourLineColor = Color.red;

    private List<Waypoint> tourWaypoints;
    private int currentWaypointIndex = 0;
    private bool waitingForAdvance = false;
    private ProximityActivator currentProximityActivator;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        StartTour();
    }

    void OnDisable()
    {
        // Clean up listener
        UnsubscribeCurrentProximity();
    }

    void Update()
    {
        // Listen for A press to advance when at a waypoint
        if (waitingForAdvance && OVRInput.GetDown(OVRInput.Button.One))
        {
            AdvanceToNextWaypoint();
        }
    }

    private void StartTour()
    {
        Debug.Log("[TourManager] Starting tour with waypoints: " + string.Join(", ", WaypointManager.Instance.Waypoints.ConvertAll(wp => wp.name)));
        tourWaypoints = WaypointManager.Instance.GetWaypointsByTourOrder();

        if (tourWaypoints.Count == 0)
        {
            Debug.LogWarning("[TourManager] No waypoints with tourOrder set.");
            return;
        }

        currentWaypointIndex = 0;

        // Set line color to tour red
        if (lineConnector != null && lineConnector.lineRenderer != null)
        {
            lineConnector.lineRenderer.startColor = tourLineColor;
            lineConnector.lineRenderer.endColor = tourLineColor;
        }

        // Show first popup
        Waypoint first = tourWaypoints[currentWaypointIndex];
        ShowTransitionPopup($"Let's start with {first.name}", "Follow the red line to the first waypoint.");
        SetTargetWaypoint(first);
    }

    private void SetTargetWaypoint(Waypoint waypoint)
    {
        // Update line connector
        GameObject visualObj = WaypointManager.Instance.GetWaypointVisual(waypoint.id);
        if (visualObj != null && lineConnector != null)
        {
            lineConnector.SetTarget(visualObj.transform);
        }

        // Update compass
        if (compass != null && visualObj != null)
        {
            compass.Waypoint = visualObj;
        }

        // Subscribe to proximity and activate
        UnsubscribeCurrentProximity();
        if (visualObj != null)
        {
            currentProximityActivator = visualObj.GetComponent<ProximityActivator>();
            if (currentProximityActivator != null)
            {
                currentProximityActivator.OnPlayerEntered += OnReachedWaypoint;
                currentProximityActivator.Activate();
            }
        }
    }

    private void OnReachedWaypoint()
    {
        // Player reached the current waypoint — they can now see the 3D model + cards
        // Wait for A press to advance
        waitingForAdvance = true;
    }

    private void AdvanceToNextWaypoint()
    {
        waitingForAdvance = false;
        UnsubscribeCurrentProximity();

        currentWaypointIndex++;

        if (currentWaypointIndex >= tourWaypoints.Count)
        {
            // Tour complete
            EndTour();
            return;
        }

        Waypoint next = tourWaypoints[currentWaypointIndex];
        ShowTransitionPopup($"Next: {next.name}", "Follow the red line to the next waypoint.");
        SetTargetWaypoint(next);
    }

    private void EndTour()
    {
        if (lineConnector != null)
        {
            lineConnector.ClearTarget();
        }

        if (popupCard != null)
        {
            popupCard.Show(
                "Tour Complete!",
                "You've visited all the waypoints. Press A to explore freely.",
                onA: () =>
                {
                    if (GameModeManager.Instance != null)
                    {
                        GameModeManager.Instance.SetMode(GameMode.FreeRoam);
                    }
                }
            );
        }
    }

    private void ShowTransitionPopup(string title, string message)
    {
        if (popupCard != null)
        {
            popupCard.Show(title, message);
        }
    }

    private void UnsubscribeCurrentProximity()
    {
        if (currentProximityActivator != null)
        {
            currentProximityActivator.OnPlayerEntered -= OnReachedWaypoint;
            currentProximityActivator.Deactivate();
            currentProximityActivator = null;
        }
    }
}
