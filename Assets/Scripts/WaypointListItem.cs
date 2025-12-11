using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaypointListItem : MonoBehaviour
{
    [Header("Main Display")]
    public TextMeshProUGUI waypointNameText;
    public Image borderImage;
    public Image activeImage;

    [Header("Edit Panel")]
    public GameObject editPanel;
    public Image colorPreviewImage;
    public Image shapePreviewImage;

    [Header("Shape Icons")]
    [Tooltip("Order must match WaypointIconType enum: Standard, POI, Warning")]
    public Sprite[] shapeIcons;

    private string waypointId;
    private bool isSelected = false;
    private bool isActiveSelect = false;

    

    public void Setup(Waypoint waypoint)
    {
        waypointId = waypoint.id;
        waypointNameText.text = waypoint.name;
        SetSelected(false);
        SetActiveSelect(false);
        SetExpanded(false);
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        if (borderImage != null)
        {
            borderImage.gameObject.SetActive(selected);
        }
    }

    public void SetActiveSelect(bool active)
    {
        isActiveSelect = active;
        if (activeImage != null)
        {
            activeImage.gameObject.SetActive(active);
        }
    }

    public void SetExpanded(bool expanded)
    {
        if (editPanel != null)
        {
            editPanel.SetActive(expanded);
        }
    }

    public void UpdateEditDisplay(Color color, WaypointIconType iconType) 
    {
        if (colorPreviewImage != null)
        {
            colorPreviewImage.color = color;
        }

        if (shapePreviewImage != null && shapeIcons != null)
        {
            int index = (int)iconType;
            if (index >= 0 && index < shapeIcons.Length && shapeIcons[index] != null)
            {
                shapePreviewImage.sprite = shapeIcons[index];
            }
        }
    }

    public string GetWaypointId()
    {
        return waypointId;
    }

}