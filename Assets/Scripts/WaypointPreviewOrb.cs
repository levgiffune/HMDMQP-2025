using UnityEngine;

public class WaypointPreviewOrb : MonoBehaviour
{
    [Header("References")]
    public Transform orbRoot;

    [Header("Visibility")]
    public float showPreviewDistance = 0f;
    public bool showOnlyWhenSelected = false;

    [Header("Preview Settings")]
    public Vector3 previewOffset = new Vector3(0.2f, 0.2f, 0f);
    public float previewScale = 0.3f;
    public float rotationSpeedDegreesPerSecond = 20f;

    private Waypoint waypointData;
    private Transform cameraTransform;
    private Transform waypointTransform;
    private GameObject previewInstance;
    private bool isSelected;
    private bool hasContent;

    public void Initialize(Waypoint waypoint, Transform playerCamera, Transform waypointRoot)
    {
        waypointData = waypoint;
        cameraTransform = playerCamera;
        waypointTransform = waypointRoot != null ? waypointRoot : transform;

        if (orbRoot == null)
        {
            GameObject rootObj = new GameObject("PreviewOrbRoot");
            rootObj.transform.SetParent(waypointTransform, false);
            orbRoot = rootObj.transform;
        }

        Refresh(waypoint);
    }

    private void Update()
    {
        UpdateVisibility();
        RotatePreview();
    }

    public void Refresh(Waypoint waypoint)
    {
        waypointData = waypoint;
        hasContent = false;

        if (waypoint == null || waypoint.previewPrefab == null)
        {
            DestroyPreview();
            SetRootActive(false);
            return;
        }

        if (previewInstance == null || previewInstance.name.StartsWith(waypoint.previewPrefab.name) == false)
        {
            DestroyPreview();
            previewInstance = Instantiate(waypoint.previewPrefab, orbRoot);
            previewInstance.transform.localPosition = previewOffset;
            previewInstance.transform.localRotation = Quaternion.identity;
            previewInstance.transform.localScale = Vector3.one * previewScale;
        }

        hasContent = true;
        UpdateVisibility();
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        UpdateVisibility();
    }

    private void RotatePreview()
    {
        if (!hasContent || orbRoot == null || !orbRoot.gameObject.activeSelf)
        {
            return;
        }

        if (rotationSpeedDegreesPerSecond <= 0f)
        {
            return;
        }

        orbRoot.Rotate(0f, rotationSpeedDegreesPerSecond * Time.deltaTime, 0f, Space.Self);
    }

    private void UpdateVisibility()
    {
        if (orbRoot == null)
        {
            return;
        }

        bool shouldShow = hasContent && (isSelected || (!showOnlyWhenSelected && IsWithinDistance(showPreviewDistance)));
        SetRootActive(shouldShow);
    }

    private bool IsWithinDistance(float maxDistance)
    {
        if (maxDistance <= 0f || cameraTransform == null || waypointData == null)
        {
            return false;
        }

        return waypointData.DistanceFrom(cameraTransform.position) <= maxDistance;
    }

    private void SetRootActive(bool active)
    {
        if (orbRoot != null && orbRoot.gameObject.activeSelf != active)
        {
            orbRoot.gameObject.SetActive(active);
        }
    }

    private void DestroyPreview()
    {
        if (previewInstance != null)
        {
            Destroy(previewInstance);
            previewInstance = null;
        }
    }
}
