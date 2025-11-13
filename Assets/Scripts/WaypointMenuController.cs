using UnityEngine;
using UnityEngine.UI;
using OVR;

public class WaypointMenuController : MonoBehaviour
{
    public Transform playerTransform;

    [Header("UI References")]
    public Transform waypointListContainer;
    public GameObject waypointListItemPrefab;
    public Button createWaypointButton;

    private int selectedIndex = -1;
    private float thumbstickCooldown = 0f;
    private const float COOLDOWN_TIME = 0.3f;

    void Start()
    {
        selectedIndex = -1; // -1 = create button
        UpdateSelection();
    }
    
    void Update()
    {
        HandleThumbstickNav();
        HandleTriggerConfirm();
        
        if (thumbstickCooldown > 0)
            thumbstickCooldown -= Time.deltaTime;
    }

    void HandleThumbstickNav()
    {
        if (thumbstickCooldown > 0) return;

        Vector2 thumbstick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);

        if (thumbstick.y > 0.5f)
        {
            selectedIndex--;
            thumbstickCooldown = COOLDOWN_TIME;
            UpdateSelection();
        } 
        else if (thumbstick.y < -0.5f)
        {
            selectedIndex++;
            thumbstickCooldown = COOLDOWN_TIME;
            UpdateSelection();
        }
    }

    void HandleTriggerConfirm()
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            ConfirmSelection();
        }
    }

    void UpdateSelection()
    {
        int itemCount = waypointListContainer.childCount;
        int totalOptions = itemCount + 1;

        if (selectedIndex < -1) selectedIndex = itemCount - 1;
        if (selectedIndex >= itemCount) selectedIndex = -1;
        
        UpdateSelectionVisuals();
    }
    
    void UpdateSelectionVisuals()
    {
        if (createWaypointButton != null)
        {
            ColorBlock colors = createWaypointButton.colors;
            colors.normalColor = (selectedIndex == -1) ? Color.yellow : Color.white;
            createWaypointButton.colors = colors;
        }

        for (int i = 0; i < waypointListContainer.childCount; i++)
        {
            WaypointListItem item = waypointListContainer.GetChild(i).GetComponent<WaypointListItem>();
            if (item != null)
            {
                item.SetSelected(i == selectedIndex);
            }
        }
    }

    void ConfirmSelection()
    {
        if (selectedIndex == -1)
        {
            CreateWaypoint();
        }
        else if (selectedIndex >= 0 && selectedIndex < waypointListContainer.childCount)
        {
            WaypointListItem item = waypointListContainer.GetChild(selectedIndex).GetComponent<WaypointListItem>();
            if (item != null)
            {
                SelectWaypoint(item.GetWaypointId());
            }
        }
    }

    void CreateWaypoint()
    {
        if (playerTransform == null)
        {
            Debug.LogError("Player transform not assigned!");
            return;
        }
        
        Vector3 spawnPosition = playerTransform.position + playerTransform.forward * 2f;
        Waypoint newWaypoint = WaypointManager.Instance.CreateWaypoint( 
            spawnPosition, 
            $"Waypoint {WaypointManager.Instance.Waypoints.Count + 1}"
        );

        AddWaypointToList(newWaypoint);
        selectedIndex = waypointListContainer.childCount - 1;
        UpdateSelection();
    }

    void SelectWaypoint(string waypointId)
    {
        Waypoint waypoint = WaypointManager.Instance.GetWaypoint(waypointId);
        if (waypoint != null)
        {
            CompassManager.Instance.Waypoint = WaypointManager.Instance.GetWaypointVisual(waypointId);
            WaypointManager.Instance.HighlightWaypoint(waypointId);
        }
    }

    void AddWaypointToList(Waypoint waypoint)
    {
        if (waypointListItemPrefab == null || waypointListContainer == null) return;
        
        GameObject itemObj = Instantiate(waypointListItemPrefab, waypointListContainer);
        WaypointListItem item = itemObj.GetComponent<WaypointListItem>();
        item.Setup(waypoint);
    }
}