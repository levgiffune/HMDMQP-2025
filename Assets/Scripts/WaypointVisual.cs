using UnityEngine;
using TMPro;
using UnityEngine.UI;

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

    [Header("Description Panel")]
    public GameObject descriptionPanel;
    public TextMeshProUGUI waypointNameText;
    public TextMeshProUGUI descriptionText;
    public RawImage infoCardImage;
    public float showInfoDistance = 0f;
    public float imageCarouselInterval = 3f;

    [Header("Floating Label")]
    public bool applyLabelOffset = false;
    public Vector3 labelOffset = new Vector3(0f, 0.3f, 0f);
    public float labelVisibleDistance = 0f;

    [Header("Optional Widgets")]
    public WaypointMediaDisplay mediaDisplay;
    public WaypointPreviewOrb previewOrb;

    private GameObject currentShapeObject;
    private MeshRenderer currentRenderer;
    private Waypoint waypointData;
    private Transform cameraTransform;
    private bool isSelected = false;
    private bool hasInfoContent = false;
    private Texture2D[] carouselImages;
    private int carouselIndex = 0;
    private float carouselTimer = 0f;

    public void Initialize(Waypoint waypoint, Transform playerCamera)
    {
        waypointData = waypoint;
        cameraTransform = playerCamera;

        transform.position = waypoint.position;
        transform.rotation = waypoint.rotation;

        if (mediaDisplay != null)
        {
            mediaDisplay.Initialize(waypoint, playerCamera, transform);
        }

        if (previewOrb != null)
        {
            previewOrb.Initialize(waypoint, playerCamera, transform);
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

        UpdateInfoCardVisibility();
        UpdateLabelVisibility();
        UpdateCarousel();
    }


    public Waypoint GetWaypointData()
    {
        return waypointData;
    }

    public void SetSelected(bool selected)
    {
        // Debug.Log(descriptionPanel);
        isSelected = selected;

        if (selectionIndicator != null)
        {
            selectionIndicator.SetActive(selected);
        }

        UpdateInfoCardVisibility();

        if (mediaDisplay != null)
        {
            mediaDisplay.SetSelected(selected);
        }

        if (previewOrb != null)
        {
            previewOrb.SetSelected(selected);
        }
    }

    public void UpdateAppearance(Waypoint waypoint)
    {
        waypointData = waypoint;

        UpdateShape(waypoint.iconType);
        UpdateColor(waypoint.color);
        RefreshLabel(waypoint);
        RefreshInfoCard(waypoint);

        if (mediaDisplay != null)
        {
            mediaDisplay.Refresh(waypoint);
        }

        if (previewOrb != null)
        {
            previewOrb.Refresh(waypoint);
        }
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

    public void GenerateDescriptionPanel(Waypoint waypoint)
    {
        RefreshInfoCard(waypoint);
    }

    private void RefreshLabel(Waypoint waypoint)
    {
        if (labelText == null || waypoint == null)
        {
            return;
        }

        labelText.text = waypoint.name;

        if (applyLabelOffset)
        {
            labelText.transform.localPosition = labelOffset;
        }
    }

    private void RefreshInfoCard(Waypoint waypoint)
    {
        if (waypoint == null)
        {
            hasInfoContent = false;
            carouselImages = null;
            UpdateInfoCardVisibility();
            return;
        }

        if (descriptionPanel == null)
        {
            hasInfoContent = false;
            carouselImages = null;
            return;
        }

        bool hasDescription = !string.IsNullOrWhiteSpace(waypoint.desc);

        // Build carousel array: images array first, fall back to single imageRef
        if (waypoint.images != null && waypoint.images.Length > 0)
        {
            carouselImages = waypoint.images;
        }
        else if (waypoint.imageRef != null)
        {
            carouselImages = new Texture2D[] { waypoint.imageRef };
        }
        else
        {
            carouselImages = null;
        }

        bool hasImage = carouselImages != null && carouselImages.Length > 0;

        if (waypointNameText != null)
        {
            waypointNameText.text = waypoint.name;
        }

        if (descriptionText != null)
        {
            descriptionText.text = hasDescription ? waypoint.desc : string.Empty;
        }

        if (infoCardImage != null)
        {
            if (hasImage)
            {
                carouselIndex = 0;
                carouselTimer = 0f;
                infoCardImage.texture = carouselImages[0];
                infoCardImage.gameObject.SetActive(true);
            }
            else
            {
                infoCardImage.texture = null;
                infoCardImage.gameObject.SetActive(false);
            }
        }

        hasInfoContent = hasDescription || hasImage;
        UpdateInfoCardVisibility();
    }

    private void UpdateCarousel()
    {
        if (carouselImages == null || carouselImages.Length <= 1 || infoCardImage == null)
        {
            return;
        }

        if (!descriptionPanel.activeSelf)
        {
            return;
        }

        carouselTimer += Time.deltaTime;
        if (carouselTimer >= imageCarouselInterval)
        {
            carouselTimer = 0f;
            carouselIndex = (carouselIndex + 1) % carouselImages.Length;
            infoCardImage.texture = carouselImages[carouselIndex];
        }
    }

    private void UpdateInfoCardVisibility()
    {
        if (descriptionPanel == null)
        {
            return;
        }

        bool shouldShow = hasInfoContent && (isSelected || IsWithinDistance(showInfoDistance));
        if (descriptionPanel.activeSelf != shouldShow)
        {
            descriptionPanel.SetActive(shouldShow);
        }
    }

    private void UpdateLabelVisibility()
    {
        if (labelText == null)
        {
            return;
        }

        if (labelVisibleDistance <= 0f || cameraTransform == null || waypointData == null)
        {
            if (!labelText.gameObject.activeSelf)
            {
                labelText.gameObject.SetActive(true);
            }
            return;
        }

        bool shouldShow = waypointData.DistanceFrom(cameraTransform.position) <= labelVisibleDistance;
        if (labelText.gameObject.activeSelf != shouldShow)
        {
            labelText.gameObject.SetActive(shouldShow);
        }
    }

    private bool IsWithinDistance(float maxDistance)
    {
        if (maxDistance <= 0f || cameraTransform == null || waypointData == null)
        {
            return false;
        }

        return waypointData.DistanceFrom(cameraTransform.position) <= maxDistance;
    }
}