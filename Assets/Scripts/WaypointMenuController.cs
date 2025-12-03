using UnityEngine;
using UnityEngine.UI;
using OVR;

public class WaypointMenuController : MonoBehaviour
{
    public static WaypointMenuController Instance { get; private set; }

    public Transform playerTransform;

    [Header("UI References")]
    public Transform waypointListContainer;
    public GameObject waypointListItemPrefab;
    public Button createWaypointButton;
    public Button deleteWaypointButton;

    private int selectedIndex = -1; // -1 = create, -2 = delete, 0+ waypoints
    private float thumbstickCooldown = 0f;
    private const float COOLDOWN_TIME = 0.3f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple WaypointMenuController instances detected. Destroying duplicate.");
            Destroy(gameObject);
        }
    }

    void Start()
    {
        selectedIndex = -1; // -1 = create button
        UpdateSelection();
    }
    
    void Update()
    {
        // Only process input when this menu is active
        if (!gameObject.activeInHierarchy) return;

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

        // Horizontal nav only works on create/delete buttons
        if (selectedIndex == -1)
        {
            if (thumbstick.x > 0.5f) // Right to delete
            {
                selectedIndex = -2;
                thumbstickCooldown = COOLDOWN_TIME;
                UpdateSelection();
            }
        }
        else if (selectedIndex == -2)
        {
            if (thumbstick.x < -0.5f) // Left to create
            {
                selectedIndex = -1;
                thumbstickCooldown = COOLDOWN_TIME;
                UpdateSelection();
            }
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

        if (selectedIndex < -2) selectedIndex = itemCount - 1;
        if (selectedIndex >= itemCount) selectedIndex = -1;
        
        UpdateSelectionVisuals();
    }
    
    void UpdateSelectionVisuals()
    {
        // Create
        if (createWaypointButton != null)
        {
            ColorBlock colors = createWaypointButton.colors;
            colors.normalColor = (selectedIndex == -1) ? Color.yellow : Color.white;
            createWaypointButton.colors = colors;
        }

        // Delete
        if (deleteWaypointButton != null)
        {
            ColorBlock colors = deleteWaypointButton.colors;
            colors.normalColor = (selectedIndex == -2) ? Color.yellow : Color.white;
            deleteWaypointButton.colors = colors;
        }

        // List Items
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
        else if (selectedIndex == -2)
        {
            DeleteSelectedWaypoint();
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

    void DeleteSelectedWaypoint()
    {
        if (waypointListContainer.childCount == 0)
        {
            Debug.Log("No waypoints to delete");
            return;
        }

        WaypointListItem firstItem = waypointListContainer.GetChild(0).GetComponent<WaypointListItem>();
        if (firstItem != null)
        {
            string wpId = firstItem.GetWaypointId();
            WaypointManager.Instance.DeleteWaypoint(wpId);
            Destroy(firstItem.gameObject);

            GameObject firstVisual = WaypointManager.Instance.GetWaypointVisual(wpId);
            Destroy(firstVisual);
        }
    }

    public void ShowMainMenu()
    {
        gameObject.SetActive(true);
        selectedIndex = -1;
        UpdateSelection();
    }

    public void OpenCustomizationMenu(string waypointId)
    {
        Waypoint waypoint = WaypointManager.Instance.GetWaypoint(waypointId);
        if (waypoint == null)
        {
            Debug.LogError($"Waypoint with ID {waypointId} not found");
            return;
        }

        CustomizationMenuController customizationMenu = FindObjectOfType<CustomizationMenuController>();
        if (customizationMenu != null)
        {
            gameObject.SetActive(false); // Hide waypoint menu
            customizationMenu.DisplayWaypointEditor(waypoint);
        }
        else
        {
            Debug.LogError("CustomizationMenuController not found in scene");
        }
    }
}