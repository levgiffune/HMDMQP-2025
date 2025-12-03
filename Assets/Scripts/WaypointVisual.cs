using UnityEngine;
using TMPro;

public class WaypointVisual : MonoBehaviour
{
    [Header("References")]
    public TextMeshPro labelText;
    public GameObject selectionIndicator;
    public Transform shapeContainer;

    [Header("Settings")]
    public bool billboardToCamera = true;

    [Header("Shape Prefabs")]
    [Tooltip("Order must match WaypointIconType enum: Standard, POI, Warning")]
    public GameObject[] shapePrefabs;

    private GameObject currentShapeObject;
    private MeshRenderer currentRenderer;
    private Waypoint waypointData;
    private Transform cameraTransform;
    private bool isSelected = false;

    public void Initialize(Waypoint waypoint, Transform playerCamera)
    {
        waypointData = waypoint;
        cameraTransform = playerCamera;

        transform.position = waypoint.position;
        transform.rotation = waypoint.rotation;

        if (labelText != null)
        {
            labelText.text = waypoint.name;
        }

        UpdateAppearance(waypoint);
    }

    private void Update()
    {
        if (billboardToCamera && cameraTransform != null)
        {
            transform.LookAt(cameraTransform);
            transform.Rotate(0, 180, 0);
        }
    }

    public Waypoint GetWaypointData()
    {
        return waypointData;
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;

        if (selectionIndicator != null)
        {
            selectionIndicator.SetActive(selected);
        }
    }

    public void UpdateAppearance(Waypoint waypoint)
    {
        waypointData = waypoint;

        UpdateShape(waypoint.iconType);
        UpdateColor(waypoint.color);
    }

    public void PreviewAppearance(Color color, WaypointIconType iconType)
    {
        UpdateShape(iconType);
        UpdateColor(color);
    }

    private void UpdateShape(WaypointIconType iconType)
    {
        int index = (int)iconType;

        if (index < 0 || index >= shapePrefabs.Length || shapePrefabs[index] == null)
        {
            Debug.LogWarning($"No prefab assigned for WaypointIconType.{iconType}");
            return;
        }

        // Skip if same shape already instantiated
        if (currentShapeObject != null && currentShapeObject.name.StartsWith(shapePrefabs[index].name))
        {
            return;
        }

        if (currentShapeObject != null)
        {
            Destroy(currentShapeObject);
        }

        Transform parent = shapeContainer != null ? shapeContainer : transform;
        currentShapeObject = Instantiate(shapePrefabs[index], parent);
        currentShapeObject.transform.localPosition = Vector3.zero;
        currentShapeObject.transform.localRotation = Quaternion.identity;

        currentRenderer = currentShapeObject.GetComponent<MeshRenderer>();
    }

    private void UpdateColor(Color color)
    {
        if (currentRenderer != null)
        {
            currentRenderer.material.color = color;
        }
    }

    public void RevertToSavedAppearance()
    {
        if (waypointData != null)
        {
            UpdateAppearance(waypointData);
        }
    }
}