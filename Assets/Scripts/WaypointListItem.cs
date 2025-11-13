using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaypointListItem : MonoBehaviour
{
    public TextMeshProUGUI waypointNameText;
    public Image borderImage;
    
    private string waypointId;
    private bool isSelected = false;

    
    public void Setup(Waypoint waypoint)
    {
        waypointId = waypoint.id;
        waypointNameText.text = waypoint.name;
        SetSelected(false);
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        if (borderImage != null)
        {
            borderImage.gameObject.SetActive(selected);
        }
    }

    public string GetWaypointId()
    {
        return waypointId;
    }

}