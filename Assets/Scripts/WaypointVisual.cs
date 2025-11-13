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

    [Header("Highlight Settings")]
    public float highlightScale = 1.5f;
    public Color highlightColor = Color.yellow;

    private Vector3 originalScale;
    private Color originalColor;
    private bool isHighlighted = false;

    void Start()
    {
        originalScale = transform.localScale;
        if (iconRenderer != null)
        {
            originalColor = iconRenderer.material.color;
        }
    }

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

    public void SetHighlighted(bool highlighted)
    {
        isHighlighted = highlighted;
        
        if (highlighted)
        {
            transform.localScale = originalScale * highlightScale;
            if (iconRenderer != null)
            {
                iconRenderer.material.color = highlightColor;
            }
        }
        else
        {
            transform.localScale = originalScale;
            if (iconRenderer != null)
            {
                iconRenderer.material.color = waypointData != null ? waypointData.color : originalColor;
            }
        }
    }
}
