using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class VRWaypointMenuCust : MonoBehaviour
{
    [Header("UI References (Assign in Inspector)")]
    public TMP_Dropdown colorDropdown;
    public TMP_Dropdown shapeDropdown;
    public TMP_InputField nameInput;
    public Button applyButton;

    [Header("Selection")]
    public WaypointVisual targetWaypointVisual; // The currently selected waypoint

    private Waypoint targetWaypoint;

    void Start()
    {
        // Hook up the Apply button
        if (applyButton != null)
            applyButton.onClick.AddListener(ApplyChanges);

        // Validate name input
        if (nameInput != null)
        {
            nameInput.onValidateInput += (string text, int charIndex, char addedChar) =>
            {
                return IsValidChar(addedChar) ? addedChar : '\0';
            };
        }
    }

    /// <summary>
    /// Sets which waypoint the menu is currently editing.
    /// Call this when selecting a waypoint in VR.
    /// </summary>
    public void SetTargetWaypoint(WaypointVisual visual)
    {
        targetWaypointVisual = visual;
        if (visual != null)
        {
            targetWaypoint = visual.GetWaypointData();

            // Populate UI with current values
            if (colorDropdown != null)
                colorDropdown.value = GetColorIndex(targetWaypoint.color);

            if (shapeDropdown != null)
                shapeDropdown.value = GetShapeIndex(targetWaypointVisual.gameObject);

            if (nameInput != null)
                nameInput.text = targetWaypoint.name;
        }
    }

    void ApplyChanges()
    {
        if (targetWaypoint == null || targetWaypointVisual == null) return;

        // Change color
        if (colorDropdown != null)
        {
            string colorText = colorDropdown.options[colorDropdown.value].text;
            if (colorText != "Choose Color") // placeholder check
            {
                Color newColor = GetSelectedColor();
                targetWaypoint.color = newColor;
                if (targetWaypointVisual.iconRenderer != null)
                    targetWaypointVisual.iconRenderer.material.color = newColor;
            }
        }

        // Change name
        if (nameInput != null)
        {
            string newName = SanitizeName(nameInput.text);
            targetWaypoint.name = newName;
            if (targetWaypointVisual.labelText != null)
                targetWaypointVisual.labelText.text = newName;
        }

        // Change shape
        if (shapeDropdown != null)
        {
            string shape = shapeDropdown.options[shapeDropdown.value].text;
            if (shape != "Choose Shape") // placeholder check
                ChangeShape(shape);
        }
    }

    Color GetSelectedColor()
    {
        switch (colorDropdown.options[colorDropdown.value].text)
        {
            case "Red": return Color.red;
            case "Green": return Color.green;
            case "Blue": return Color.blue;
            case "Yellow": return Color.yellow;
            default: return Color.white;
        }
    }

    int GetColorIndex(Color c)
    {
        if (c == Color.red) return 1;
        if (c == Color.green) return 2;
        if (c == Color.blue) return 3;
        if (c == Color.yellow) return 4;
        return 0; // placeholder index
    }

    int GetShapeIndex(GameObject obj)
    {
        string name = obj.name.ToLower();
        if (name.Contains("sphere")) return 1;
        if (name.Contains("cube")) return 2;
        if (name.Contains("capsule")) return 3;
        return 0; // placeholder index
    }

    void ChangeShape(string shape)
    {
        if (targetWaypointVisual == null || targetWaypoint == null) return;

        GameObject oldObj = targetWaypointVisual.gameObject;
        Vector3 pos = oldObj.transform.position;
        Vector3 scale = oldObj.transform.localScale;
        Color color = targetWaypoint.color;
        string name = targetWaypoint.name;

        // Detach label temporarily
        TMP_Text label = targetWaypointVisual.labelText;
        if (label != null)
            label.transform.parent = null;

        GameObject newObj = null;
        switch (shape)
        {
            case "Sphere": newObj = GameObject.CreatePrimitive(PrimitiveType.Sphere); break;
            case "Cube": newObj = GameObject.CreatePrimitive(PrimitiveType.Cube); break;
            case "Capsule": newObj = GameObject.CreatePrimitive(PrimitiveType.Capsule); break;
        }

        if (newObj != null)
        {
            newObj.transform.position = pos;
            newObj.transform.localScale = scale;
            newObj.name = name;

            MeshRenderer meshRend = newObj.GetComponent<MeshRenderer>();
            if (meshRend != null)
                meshRend.material.color = color;

            WaypointVisual visual = newObj.AddComponent<WaypointVisual>();
            visual.iconRenderer = meshRend;
            visual.labelText = label;

            // Reparent label
            if (label != null)
            {
                label.transform.parent = newObj.transform;
                label.transform.localPosition = new Vector3(0, 0.5f, 0);
            }

            // Copy Waypoint data
            visual.Initialize(targetWaypoint, Camera.main.transform);

            // Replace old object
            Destroy(oldObj);

            targetWaypointVisual = visual;
        }
    }

    string SanitizeName(string input)
    {
        char[] validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_".ToCharArray();
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        foreach (char c in input)
            if (System.Array.Exists(validChars, e => e == c))
                sb.Append(c);

        if (sb.Length == 0) sb.Append("Waypoint");
        return sb.ToString();
    }

    bool IsValidChar(char c)
    {
        char[] validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_".ToCharArray();
        return System.Array.Exists(validChars, e => e == c);
    }
}
