using UnityEngine;
using TMPro;

/// <summary>
/// Info card attached near the 3D model (WaypointModel).
/// Shows model name and description.
/// Shown/hidden by ProximityActivator via WaypointVisual.
/// </summary>
public class InfoCard : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;

    public void Initialize(Waypoint waypoint)
    {
        if (nameText != null) nameText.text = waypoint.modelName ?? waypoint.name;
        if (descriptionText != null) descriptionText.text = waypoint.modelDesc ?? "";
    }
}
