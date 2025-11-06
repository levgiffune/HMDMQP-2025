using UnityEngine;
using TMPro;

public class WaypointVisual : MonoBehaviour
{
    [Header("References")]
    public MeshRenderer iconRenderer; 
    public TextMeshPro labelText;

    [Header("Settings")]
    public bool billboardToCamera = true; 
    
    private Waypoint waypointData;
    private Transform cameraTransform;

    public void Initialize(Waypoint waypoint, Transform playerCamera)
    {
        waypointData = waypoint;
        cameraTransform = playerCamera;

        transform.position = waypoint.position;
        transform.rotation = waypoint.rotation;

        if (iconRenderer != null)
        {
            iconRenderer.material.color = waypoint.color;
        }

        if (labelText != null)
        {
            labelText.text = waypoint.name;
        }
    }

    void Update()
    {
        if (billboardToCamera && cameraTransform != null)
        {
            // Face camera
            transform.LookAt(cameraTransform);
            transform.Rotate(0, 180, 0); // Flip to face correct direction
        }
    }
    
    public Waypoint GetWaypointData()
    {
        return waypointData;
    }
}
