using UnityEngine;
using TMPro;

public class WaypointVisual : MonoBehaviour
{
    [Header("References")]
    public TextMeshPro labelText;
    public Transform shapeContainer;

    [Header("Diamond")]
    public GameObject diamondPrefab;

    [Header("Description Panel")]
    public DescriptionCard descriptionCard;

    [Header("Widgets")]
    public WaypointModel waypointModel;
    public InfoCard infoCard;

    [Header("Settings")]
    public bool billboardToCamera = true;

    private GameObject currentDiamond;
    private MeshRenderer diamondRenderer;
    private Waypoint waypointData;
    private Transform cameraTransform;

    public void Initialize(Waypoint waypoint, Transform playerCamera)
    {
        waypointData = waypoint;
        cameraTransform = playerCamera;

        transform.position = waypoint.position;
        transform.rotation = waypoint.rotation;

        // Set label
        if (labelText != null)
        {
            labelText.text = waypoint.name;
        }

        // Create diamond shape
        CreateDiamond(waypoint.color);

        // Initialize description card (hidden by default)
        if (descriptionCard != null)
        {
            descriptionCard.Initialize(waypoint);
            descriptionCard.gameObject.SetActive(false);
        }

        // Initialize model
        if (waypointModel != null)
        {
            waypointModel.Initialize(waypoint, playerCamera, transform);
        }

        // Initialize InfoCard with model data (only if model exists)
        if (infoCard != null && !string.IsNullOrEmpty(waypoint.model))
        {
            infoCard.Initialize(waypoint);
            infoCard.gameObject.SetActive(false);
        }
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

    /// <summary>
    /// Show or hide proximity content (description panel, model, InfoCard).
    /// Called by ProximityActivator.
    /// </summary>
    public void SetProximityActive(bool active)
    {
        if (descriptionCard != null && waypointData?.desc != null)
        {
            descriptionCard.gameObject.SetActive(active);
        }

        if (waypointModel != null)
        {
            if (active) waypointModel.Show();
            else waypointModel.Hide();
        }

        if (infoCard != null && waypointModel != null && waypointModel.HasModel)
        {
            infoCard.gameObject.SetActive(active);
        }
    }

    private void CreateDiamond(Color color)
    {
        if (diamondPrefab == null || shapeContainer == null) return;

        if (currentDiamond != null)
        {
            Destroy(currentDiamond);
        }

        currentDiamond = Instantiate(diamondPrefab, shapeContainer);
        currentDiamond.transform.localPosition = Vector3.zero;
        currentDiamond.transform.localRotation = Quaternion.identity;

        diamondRenderer = currentDiamond.GetComponent<MeshRenderer>();
        if (diamondRenderer != null)
        {
            diamondRenderer.material.color = color;
        }
    }
}