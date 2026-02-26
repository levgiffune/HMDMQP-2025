using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Simple list entry for the Free Roam waypoint list.
/// </summary>
public class FreeRoamListEntry : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public Image highlightImage;
    public Image activeImage;

    private string waypointId;
    public string WaypointId => waypointId;

    public void Setup(Waypoint waypoint)
    {
        waypointId = waypoint.id;
        if (nameText != null) nameText.text = waypoint.name;
        SetHighlighted(false);
        SetActiveSelect(false);
    }

    public void SetHighlighted(bool highlighted)
    {
        if (highlightImage != null)
        {
            highlightImage.gameObject.SetActive(highlighted);
        }
    }

    public void SetActiveSelect(bool active)
    {
        if (activeImage != null)
        {
            activeImage.gameObject.SetActive(active);
        }
    }
}
