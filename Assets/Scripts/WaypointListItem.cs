using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaypointListItem : MonoBehaviour
{
    public TextMeshProUGUI waypointNameText;
    
    private string waypointId;
    
    public void Setup(Waypoint waypoint)
    {
        waypointId = waypoint.id;
        waypointNameText.text = waypoint.name;
    }

}