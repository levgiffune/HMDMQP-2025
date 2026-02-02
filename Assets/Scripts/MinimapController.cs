using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapController : MonoBehaviour
{
    public static MinimapController Instance { get; private set; }

    [Header("References")]
    public Transform playerCamera;
    public RectTransform minimapBackground;
    public RectTransform playerIcon;
    public RectTransform waypointIconsContainer;
    public GameObject waypointIconPrefab;

    [Header("Minimap Settings")]
    public float minimapRadius = 100f;
    public float worldRadius = 20f;

    [Header("Position Settings")]
    public float distanceFromCamera = 0.5f;
    public float heightOffset = 0.1f;
    public float horizontalOffset = 0.3f;

    private Canvas canvas;
    private Dictionary<string, RectTransform> waypointIcons = new Dictionary<string, RectTransform>();

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

    void Start()
    {
        canvas = GetComponent<Canvas>();
        
        if (playerCamera == null)
        {
            playerCamera = Camera.main.transform;
        }
    }

    void LateUpdate()
    {
        PositionMinimap();
        UpdatePlayerIcon();
        UpdateWaypointIcons();
    }

    void PositionMinimap()
    {
        if (playerCamera == null) return;

        Vector3 targetPosition = playerCamera.position 
            + playerCamera.forward * distanceFromCamera 
            + playerCamera.right * horizontalOffset
            + Vector3.up * heightOffset;

        transform.position = targetPosition;

        transform.LookAt(playerCamera);
        transform.Rotate(0, 180, 0);
    }

    void UpdatePlayerIcon()
    {
        if (playerCamera == null || playerIcon == null) return;

        float yRotation = playerCamera.eulerAngles.y;
        playerIcon.localRotation = Quaternion.Euler(0, 0, -yRotation);
    }

    void UpdateWaypointIcons()
    {
        if (WaypointManager.Instance == null) return;

        foreach (Waypoint waypoint in WaypointManager.Instance.Waypoints)
        {
            if (!waypointIcons.ContainsKey(waypoint.id))
            {
                CreateWaypointIcon(waypoint);
            }

            UpdateWaypointIconPosition(waypoint);
        }

        CleanupDeletedWaypoints();
    }

    void CreateWaypointIcon(Waypoint waypoint)
    {
        GameObject iconObj = Instantiate(waypointIconPrefab, waypointIconsContainer);
        RectTransform iconRect = iconObj.GetComponent<RectTransform>();
        
        Image iconImage = iconObj.GetComponent<Image>();
        if (iconImage != null)
        {
            iconImage.color = waypoint.color;
        }

        waypointIcons.Add(waypoint.id, iconRect);
    }

    public void CreateWaypointIconPublic(Waypoint waypoint)
    {
        GameObject iconObj = Instantiate(waypointIconPrefab, waypointIconsContainer);
        RectTransform iconRect = iconObj.GetComponent<RectTransform>();
        
        Image iconImage = iconObj.GetComponent<Image>();
        if (iconImage != null)
        {
            iconImage.color = waypoint.color;
        }

        waypointIcons.Add(waypoint.id, iconRect);
    }

    void UpdateWaypointIconPosition(Waypoint waypoint)
    {
        if (!waypointIcons.ContainsKey(waypoint.id)) return;

        RectTransform iconRect = waypointIcons[waypoint.id];

        Vector3 playerPos = playerCamera.position;
        Vector3 waypointPos = waypoint.position;

        float deltaX = waypointPos.x - playerPos.x;
        float deltaZ = waypointPos.z - playerPos.z;

        float distance = Mathf.Sqrt(deltaX * deltaX + deltaZ * deltaZ);
        float angle = Mathf.Atan2(deltaX, deltaZ) * Mathf.Rad2Deg;

        float clampedDistance = Mathf.Min(distance, worldRadius);
        float minimapDistance = (clampedDistance / worldRadius) * minimapRadius;

        float angleRad = angle * Mathf.Deg2Rad;
        float iconX = Mathf.Sin(angleRad) * minimapDistance;
        float iconY = Mathf.Cos(angleRad) * minimapDistance;

        iconRect.anchoredPosition = new Vector2(iconX, iconY);
    }

    void CleanupDeletedWaypoints()
    {
        List<string> toRemove = new List<string>();

        foreach (string id in waypointIcons.Keys)
        {
            if (WaypointManager.Instance.GetWaypoint(id) == null)
            {
                toRemove.Add(id);
            }
        }

        foreach (string id in toRemove)
        {
            Destroy(waypointIcons[id].gameObject);
            waypointIcons.Remove(id);
        }
    }

    public void RefreshWaypointIcon(Waypoint waypoint)
    {
        if (waypointIcons.ContainsKey(waypoint.id))
        {
            Image iconImage = waypointIcons[waypoint.id].GetComponent<Image>();
            if (iconImage != null)
            {
                iconImage.color = waypoint.color;
            }
        }
    }
}