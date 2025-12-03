using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomizationMenuController : MonoBehaviour
{
    [Header("UI References")]
    public Button backButton;
    public Button colorButton;
    public Button shapeButton;
    public TextMeshProUGUI waypointNameDisplay;

    private int selectedIndex = 0; // 0=back, 1=color, 2=shape
    private Color[] colors = { Color.red, Color.blue, Color.green, new Color(0.5f, 0f, 0.5f) }; // purple
    private int currentColorIndex = 0;
    private int currentShapeIndex = 0;

    private Waypoint currentWaypoint;
    private float thumbstickCooldown = 0f;
    private const float COOLDOWN_TIME = 0.3f;

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
        // Wrap around
        if (selectedIndex < 0) selectedIndex = 2;
        if (selectedIndex > 2) selectedIndex = 0;
        
        // Update visuals
        UpdateButtonVisuals(backButton, selectedIndex == 0);
        UpdateButtonVisuals(colorButton, selectedIndex == 1);
        UpdateButtonVisuals(shapeButton, selectedIndex == 2);
    }

    void UpdateButtonVisuals(Button button, bool isSelected)
    {
        if (button != null)
        {
            ColorBlock colors = button.colors;
            colors.normalColor = isSelected ? Color.yellow : Color.white;
            button.colors = colors;
        }
    }

    void ConfirmSelection()
    {
        switch (selectedIndex)
        {
            case 0: // Back
                OnBackButton();
                break;
            case 1: // Color
                CycleColor();
                break;
            case 2: // Shape
                CycleShape();
                break;
        }
    }

    void CycleColor()
    {
        if (currentWaypoint == null) return;
        
        currentColorIndex = (currentColorIndex + 1) % colors.Length;
        currentWaypoint.color = colors[currentColorIndex];
        
        // Update visual in real-time
        WaypointManager.Instance.UpdateWaypointVisual(currentWaypoint.id);
    }

    void CycleShape()
    {
        if (currentWaypoint == null) return;
        
        currentShapeIndex = (currentShapeIndex + 1) % 3;
        currentWaypoint.shape = (WaypointShape)currentShapeIndex;
        
        // Update visual in real-time
        WaypointManager.Instance.UpdateWaypointVisual(currentWaypoint.id);
    }


    public void DisplayWaypointEditor(Waypoint waypoint)
    {
        currentWaypoint = waypoint;
        gameObject.SetActive(true);
        
        // Set current indices based on waypoint
        currentColorIndex = System.Array.IndexOf(colors, waypoint.color);
        if (currentColorIndex < 0) currentColorIndex = 0;
        currentShapeIndex = (int)waypoint.shape;
        
        selectedIndex = 0;
        UpdateSelection();
        
        if (waypointNameDisplay != null)
        {
            waypointNameDisplay.text = $"Editing: {waypoint.name}";
        }
    }

    void OnBackButton()
    {
        gameObject.SetActive(false);
        WaypointMenuController.Instance.ShowMainMenu();
    }

    public Waypoint GetCurrentWaypoint()
    {
        return currentWaypoint;
    }
}
