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

    [Header("Layout")]
    public float descriptionCardSideOffset = 1.5f;
    public float infoCardSideOffset = 1.5f;

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

        // If descriptionCard is a prefab asset reference (not a scene instance), instantiate it
        if (descriptionCard != null && !descriptionCard.gameObject.scene.IsValid())
        {
            GameObject cardGo = Instantiate(descriptionCard.gameObject);
            cardGo.transform.position = waypoint.position;
            descriptionCard = cardGo.GetComponent<DescriptionCard>();
        }

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

        // If infoCard is a prefab asset reference, instantiate it (only if a model exists)
        if (infoCard != null && !infoCard.gameObject.scene.IsValid() && !string.IsNullOrEmpty(waypoint.model))
        {
            GameObject cardGo = Instantiate(infoCard.gameObject);
            cardGo.transform.position = waypoint.position;
            infoCard = cardGo.GetComponent<InfoCard>();
        }

        if (infoCard != null && !string.IsNullOrEmpty(waypoint.model))
        {
            infoCard.Initialize(waypoint);
            infoCard.gameObject.SetActive(false);
        }

        // Re-parent ModelRoot under InfoCard, retaining world scale and offset
        if (waypointModel != null && infoCard != null && waypointModel.HasModel)
        {
            Transform root = waypointModel.ModelRootTransform;
            Vector3 worldScale = root.lossyScale;
            Vector3 localPos = root.localPosition; // modelOffset
            root.SetParent(infoCard.transform, false);
            // Compensate for InfoCard's canvas scale
            Vector3 parentScale = infoCard.transform.lossyScale;
            root.localScale = new Vector3(
                worldScale.x / parentScale.x,
                worldScale.y / parentScale.y,
                worldScale.z / parentScale.z
            );
            // Compensate position so modelOffset works in world units
            root.localPosition = new Vector3(
                localPos.x / parentScale.x,
                localPos.y / parentScale.y,
                localPos.z / parentScale.z
            );
        }

        // Initialize proximity activator
        ProximityActivator proximityActivator = GetComponent<ProximityActivator>();
        if (proximityActivator != null)
        {
            proximityActivator.Initialize(waypoint, playerCamera);
        }
    }

    private void Update()
    {
        if (billboardToCamera && cameraTransform != null)
        {
            transform.LookAt(cameraTransform);
            transform.Rotate(0, 180, 0);

            // Horizontal camera-right vector for left/right card offsets
            Vector3 camRight = cameraTransform.right;
            camRight.y = 0f;
            if (camRight.sqrMagnitude > 0.001f) camRight.Normalize();

            if (descriptionCard != null)
            {
                descriptionCard.transform.position = transform.position + camRight * descriptionCardSideOffset;
                if (descriptionCard.gameObject.activeSelf)
                {
                    descriptionCard.transform.LookAt(cameraTransform);
                    descriptionCard.transform.Rotate(0, 180, 0);
                }
            }

            if (infoCard != null)
            {
                infoCard.transform.position = transform.position - camRight * infoCardSideOffset;
                if (infoCard.gameObject.activeSelf)
                {
                    infoCard.transform.LookAt(cameraTransform);
                    infoCard.transform.Rotate(0, 180, 0);
                }
            }
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