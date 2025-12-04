using UnityEngine;
using UnityEngine.UI;

public class WaypointMenuController : MonoBehaviour
{
    public static WaypointMenuController Instance { get; private set; }

    public Transform playerTransform;

    [Header("UI References")]
    public VRMenu vrMenu;
    public Transform waypointListContainer;
    public GameObject waypointListItemPrefab;
    public Button createWaypointButton;
    public Button deleteWaypointButton;
    public Button editWaypointButton;

    [Header("Customization Options")]
    public Color[] availableColors = { Color.red, Color.blue, Color.green, Color.magenta, Color.cyan };

    [Header("Pagination")]
    public int itemsPerPage = 4;
    private int currentPage = 0;

    // -1 = create, -2 = delete, -3 = edit, 0+ = waypoints
    private int selectedIndex = -1;
    private float thumbstickCooldown = 0f;
    private const float COOLDOWN_TIME = 0.3f;

    // Currently selected waypoint (for compass and editing)
    private string selectedWaypointId = null;
    private WaypointVisual currentlySelectedVisual = null;
    private WaypointListItem currentlySelectedListItem = null;

    // Editing state
    private bool isEditing = false;
    private Waypoint editingWaypoint = null;
    private WaypointVisual editingVisual = null;
    private WaypointListItem editingListItem = null;

    // Preview values
    private int previewColorIndex = 0;
    private int previewShapeIndex = 0;

    // Original values for reverting
    private Color originalColor;
    private WaypointIconType originalIconType;

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
        selectedIndex = -1;
        UpdateSelection();
    }

    void Update()
    {
        if (vrMenu != null && !vrMenu.IsOpen) return;

        if (isEditing)
        {
            HandleEditingInput();
        }
        else
        {
            HandleThumbstickNav();
            HandleTriggerConfirm();
        }

        if (thumbstickCooldown > 0)
            thumbstickCooldown -= Time.deltaTime;
    }

    void HandleThumbstickNav()
    {
        if (thumbstickCooldown > 0) return;

        Vector2 thumbstick = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);

        // Vertical navigation
        if (thumbstick.y > 0.5f)
        {
            selectedIndex--;
            thumbstickCooldown = COOLDOWN_TIME;
            
            // Check if we need to go to previous page
            if (selectedIndex >= 0)
            {
                int selectedPage = selectedIndex / itemsPerPage;
                if (selectedPage < currentPage)
                {
                    currentPage = selectedPage;
                    UpdatePageDisplay();
                }
            }
            else
            {
                // Going back to buttons, show first page
                currentPage = 0;
                UpdatePageDisplay();
            }
            
            UpdateSelection();
        }
        else if (thumbstick.y < -0.5f)
        {
            selectedIndex++;
            thumbstickCooldown = COOLDOWN_TIME;
            
            // Check if we need to go to next page
            int itemCount = waypointListContainer.childCount;
            if (selectedIndex < itemCount)
            {
                int selectedPage = selectedIndex / itemsPerPage;
                if (selectedPage > currentPage)
                {
                    currentPage = selectedPage;
                    UpdatePageDisplay();
                }
            }
            
            UpdateSelection();
        }

        // Horizontal navigation between buttons
        if (selectedIndex == -1)
        {
            if (thumbstick.x > 0.5f)
            {
                selectedIndex = -2;
                thumbstickCooldown = COOLDOWN_TIME;
                UpdateSelection();
            }
        }
        else if (selectedIndex == -2)
        {
            if (thumbstick.x < -0.5f)
            {
                selectedIndex = -1;
                thumbstickCooldown = COOLDOWN_TIME;
                UpdateSelection();
            }
            else if (thumbstick.x > 0.5f)
            {
                selectedIndex = -3;
                thumbstickCooldown = COOLDOWN_TIME;
                UpdateSelection();
            }
        }
        else if (selectedIndex == -3)
        {
            if (thumbstick.x < -0.5f)
            {
                selectedIndex = -2;
                thumbstickCooldown = COOLDOWN_TIME;
                UpdateSelection();
            }
        }
    }

    void HandleTriggerConfirm()
    {
        if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
        {
            ConfirmSelection();
        }
    }

    void HandleEditingInput()
    {
        if (thumbstickCooldown > 0) return;

        Vector2 thumbstick = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);

        // Left/right cycles color
        if (thumbstick.x > 0.5f)
        {
            previewColorIndex = (previewColorIndex + 1) % availableColors.Length;
            thumbstickCooldown = COOLDOWN_TIME;
            UpdatePreview();
        }
        else if (thumbstick.x < -0.5f)
        {
            previewColorIndex = (previewColorIndex - 1 + availableColors.Length) % availableColors.Length;
            thumbstickCooldown = COOLDOWN_TIME;
            UpdatePreview();
        }

        // Up/down cycles shape
        if (thumbstick.y > 0.5f)
        {
            int shapeCount = System.Enum.GetValues(typeof(WaypointIconType)).Length;
            previewShapeIndex = (previewShapeIndex - 1 + shapeCount) % shapeCount;
            thumbstickCooldown = COOLDOWN_TIME;
            UpdatePreview();
        }
        else if (thumbstick.y < -0.5f)
        {
            int shapeCount = System.Enum.GetValues(typeof(WaypointIconType)).Length;
            previewShapeIndex = (previewShapeIndex + 1) % shapeCount;
            thumbstickCooldown = COOLDOWN_TIME;
            UpdatePreview();
        }

        // Trigger confirms
        if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
        {
            ConfirmEdit();
        }

        // B button cancels
        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            CancelEdit();
        }
    }

    void UpdatePreview()
    {
        Color previewColor = availableColors[previewColorIndex];
        WaypointIconType previewIconType = (WaypointIconType)previewShapeIndex;

        if (editingVisual != null)
        {
            editingVisual.PreviewAppearance(previewColor, previewIconType);
        }

        if (editingListItem != null)
        {
            editingListItem.UpdateEditDisplay(previewColor, previewIconType);
        }
    }

    void EnterEditMode()
    {
        if (string.IsNullOrEmpty(selectedWaypointId))
        {
            Debug.Log("No waypoint selected to edit");
            return;
        }

        editingWaypoint = WaypointManager.Instance.GetWaypoint(selectedWaypointId);
        if (editingWaypoint == null) return;

        GameObject visualObj = WaypointManager.Instance.GetWaypointVisual(selectedWaypointId);
        if (visualObj == null) return;

        editingVisual = visualObj.GetComponent<WaypointVisual>();

        // Find the list item for this waypoint
        editingListItem = FindListItemById(selectedWaypointId);

        // Cache original values
        originalColor = editingWaypoint.color;
        originalIconType = editingWaypoint.iconType;

        // Set preview indices to match current values
        previewColorIndex = System.Array.IndexOf(availableColors, originalColor);
        if (previewColorIndex < 0) previewColorIndex = 0;
        previewShapeIndex = (int)originalIconType;

        // Expand the list item
        if (editingListItem != null)
        {
            editingListItem.SetExpanded(true);
            editingListItem.UpdateEditDisplay(originalColor, originalIconType);
        }

        isEditing = true;
    }

    WaypointListItem FindListItemById(string waypointId)
    {
        for (int i = 0; i < waypointListContainer.childCount; i++)
        {
            WaypointListItem item = waypointListContainer.GetChild(i).GetComponent<WaypointListItem>();
            if (item != null && item.GetWaypointId() == waypointId)
            {
                return item;
            }
        }
        return null;
    }

    void ConfirmEdit()
    {
        if (editingWaypoint != null)
        {
            editingWaypoint.color = availableColors[previewColorIndex];
            editingWaypoint.iconType = (WaypointIconType)previewShapeIndex;

            if (editingVisual != null)
            {
                editingVisual.UpdateAppearance(editingWaypoint);
            }
        }

        ExitEditMode();
    }

    void CancelEdit()
    {
        if (editingVisual != null)
        {
            editingVisual.PreviewAppearance(originalColor, originalIconType);
        }

        ExitEditMode();
    }

    void ExitEditMode()
    {
        if (editingListItem != null)
        {
            editingListItem.SetExpanded(false);
        }

        isEditing = false;
        editingWaypoint = null;
        editingVisual = null;
        editingListItem = null;
    }

    void UpdateSelection()
    {
        int itemCount = waypointListContainer.childCount;

        if (selectedIndex < -3) selectedIndex = itemCount - 1;
        if (selectedIndex >= itemCount) selectedIndex = -1;

        UpdateSelectionVisuals();   
        Debug.Log("UpdateSelection called");
    }

    void UpdateSelectionVisuals()
    {
        // Create button
        if (createWaypointButton != null)
        {
            ColorBlock colors = createWaypointButton.colors;
            colors.normalColor = (selectedIndex == -1) ? Color.yellow : Color.white;
            createWaypointButton.colors = colors;
        }

        // Delete button
        if (deleteWaypointButton != null)
        {
            ColorBlock colors = deleteWaypointButton.colors;
            colors.normalColor = (selectedIndex == -2) ? Color.yellow : Color.white;
            deleteWaypointButton.colors = colors;
        }

        // Edit button
        if (editWaypointButton != null)
        {
            ColorBlock colors = editWaypointButton.colors;
            colors.normalColor = (selectedIndex == -3) ? Color.yellow : Color.white;
            editWaypointButton.colors = colors;
        }

        // List items
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
        else if (selectedIndex == -3)
        {
            EnterEditMode();
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
        SelectWaypoint(newWaypoint.id);
        UpdateSelection();
    }

    void SelectWaypoint(string waypointId)
    {
        // Deselect previous
        if (currentlySelectedVisual != null)
        {
            currentlySelectedVisual.SetSelected(false);
        }

        if (currentlySelectedListItem != null)
        {
            currentlySelectedListItem.SetActiveSelect(false);
        }

        selectedWaypointId = waypointId;

        Waypoint waypoint = WaypointManager.Instance.GetWaypoint(waypointId);
        
        if (waypoint != null)
        {
            GameObject visualObj = WaypointManager.Instance.GetWaypointVisual(waypointId);
            WaypointListItem item = FindListItemById(waypointId);

            if (visualObj != null)
            {
                CompassManager.Instance.Waypoint = visualObj;

                WaypointVisual visual = visualObj.GetComponent<WaypointVisual>();
                if (visual != null)
                {
                    visual.SetSelected(true);
                    currentlySelectedVisual = visual;
                }
            }

            if (item != null)
            {
                item.SetActiveSelect(true);
                currentlySelectedListItem = item;
            }
        }
    }

    void AddWaypointToList(Waypoint waypoint)
    {
        if (waypointListItemPrefab == null || waypointListContainer == null) return;

        GameObject itemObj = Instantiate(waypointListItemPrefab, waypointListContainer);
        WaypointListItem item = itemObj.GetComponent<WaypointListItem>();
        item.Setup(waypoint);

        UpdatePageDisplay();
    }

    public void AddWaypointToListPublic(Waypoint waypoint)
        {
            if (waypointListItemPrefab == null || waypointListContainer == null) return;

            GameObject itemObj = Instantiate(waypointListItemPrefab, waypointListContainer);
            WaypointListItem item = itemObj.GetComponent<WaypointListItem>();
            item.Setup(waypoint);

            UpdatePageDisplay();
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

            if (wpId == selectedWaypointId)
            {
                selectedWaypointId = null;
                currentlySelectedVisual = null;
            }

            WaypointManager.Instance.DeleteWaypoint(wpId);
            Destroy(firstItem.gameObject);

            GameObject firstVisual = WaypointManager.Instance.GetWaypointVisual(wpId);
            Destroy(firstVisual);

            // Recalculate page if needed
            int pageCount = GetPageCount();
            if (currentPage >= pageCount && currentPage > 0)
            {
                currentPage--;
            }
            UpdatePageDisplay();
        }
    }

    int GetPageCount()
    {
        int itemCount = waypointListContainer.childCount;
        if (itemCount == 0) return 1;
        return Mathf.CeilToInt((float)itemCount / itemsPerPage);
    }

    void UpdatePageDisplay()
    {
        int itemCount = waypointListContainer.childCount;
        
        for (int i = 0; i < itemCount; i++)
        {
            GameObject item = waypointListContainer.GetChild(i).gameObject;
            int itemPage = i / itemsPerPage;
            item.SetActive(itemPage == currentPage);
        }
    }

    void NextPage()
    {
        int pageCount = GetPageCount();
        if (currentPage < pageCount - 1)
        {
            currentPage++;
            UpdatePageDisplay();
        }
    }

    void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            UpdatePageDisplay();
        }
    }

    public void ShowMainMenu()
    {
        gameObject.SetActive(true);
        selectedIndex = -1;
        UpdateSelection();
    }
}