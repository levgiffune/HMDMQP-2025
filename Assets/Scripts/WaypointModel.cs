using UnityEngine;

/// <summary>
/// Spawns a 3D model from Resources at a fixed offset from the waypoint.
/// The model faces the player each frame (Y-axis billboard).
/// Replaces the old WaypointPreviewOrb system.
/// Shown/hidden by ProximityActivator via WaypointVisual.
/// </summary>
public class WaypointModel : MonoBehaviour
{
    [Header("Settings")]
    public Vector3 modelOffset = new Vector3(1f, 0f, 0f);
    public float modelScale = 20f;
    public Vector3 modelRotationOffset = new Vector3(-90f, 0f, 0f);

    private Transform modelRoot;
    private GameObject modelInstance;
    private Transform cameraTransform;
    private bool hasModel;

    public void Initialize(Waypoint waypoint, Transform playerCamera, Transform waypointRoot)
    {
        cameraTransform = playerCamera;

        if (waypoint == null || string.IsNullOrEmpty(waypoint.model))
        {
            hasModel = false;
            SetRootActive(false);
            return;
        }

        GameObject prefab = Resources.Load<GameObject>(waypoint.model);
        if (prefab == null)
        {
            Debug.LogWarning($"WaypointModel: Could not find prefab '{waypoint.model}' in Resources.");
            hasModel = false;
            SetRootActive(false);
            return;
        }

        // Create a root container parented to the waypoint
        GameObject rootObj = new GameObject("ModelRoot");
        rootObj.transform.SetParent(waypointRoot != null ? waypointRoot : transform, false);
        rootObj.transform.localPosition = modelOffset;
        modelRoot = rootObj.transform;

        // Instantiate the model prefab
        modelInstance = Instantiate(prefab, modelRoot);
        modelInstance.transform.localPosition = Vector3.zero;
        modelInstance.transform.localRotation = Quaternion.Euler(modelRotationOffset);
        modelInstance.transform.localScale = Vector3.one * modelScale;

        hasModel = true;

        // Start hidden — ProximityActivator will show us
        SetRootActive(false);
    }

    private void Update()
    {
        if (!hasModel || cameraTransform == null || modelRoot == null) return;
        if (!modelRoot.gameObject.activeSelf) return;

        // Billboard: rotate model root to face the player on Y axis only
        Vector3 lookDir = cameraTransform.position - modelRoot.position;
        lookDir.y = 0f;
        if (lookDir.sqrMagnitude > 0.001f)
        {
            modelRoot.rotation = Quaternion.LookRotation(lookDir);
        }
    }

    /// <summary>
    /// Show the model. Called by WaypointVisual.SetProximityActive().
    /// </summary>
    public void Show()
    {
        if (hasModel)
        {
            SetRootActive(true);
        }
    }

    /// <summary>
    /// Hide the model.
    /// </summary>
    public void Hide()
    {
        SetRootActive(false);
    }

    public bool HasModel => hasModel;
    public Transform ModelRootTransform => modelRoot;

    private void SetRootActive(bool active)
    {
        if (modelRoot != null && modelRoot.gameObject.activeSelf != active)
        {
            modelRoot.gameObject.SetActive(active);
        }
    }
}
