using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Simple read-only waypoint list for Free Roam mode.
/// Thumbstick to navigate, A to select, Trigger to open/close (via VRMenuToggle).
/// </summary>
public class FreeRoamMenu : MonoBehaviour
{
    [Header("References")]
    public Transform listContainer;
    public GameObject listItemPrefab;
    public WaypointLineConnector lineConnector;
    public CompassManager compass;

    [Header("Settings")]
    public int itemsPerPage = 4;

    private int selectedIndex = 0;
    private int currentPage = 0;
    private float thumbstickCooldown = 0f;
    private const float COOLDOWN_TIME = 0.3f;

    private List<FreeRoamListEntry> listEntries = new List<FreeRoamListEntry>();
    private string activeWaypointId = null;

    void OnEnable()
    {
        PopulateList();
    }

    void Update()
    {
        HandleThumbstickNav();
        HandleConfirm();

        if (thumbstickCooldown > 0)
            thumbstickCooldown -= Time.deltaTime;
    }

    private void PopulateList()
    {
        // Clear existing
        foreach (Transform child in listContainer)
        {
            Destroy(child.gameObject);
        }
        listEntries.Clear();

        if (WaypointManager.Instance == null) return;

        foreach (Waypoint wp in WaypointManager.Instance.Waypoints)
        {
            GameObject itemObj = Instantiate(listItemPrefab, listContainer);
            FreeRoamListEntry entry = itemObj.GetComponent<FreeRoamListEntry>();
            if (entry == null)
            {
                entry = itemObj.AddComponent<FreeRoamListEntry>();
            }
            entry.Setup(wp);
            listEntries.Add(entry);
        }

        selectedIndex = 0;
        currentPage = 0;
        UpdatePageDisplay();
        UpdateSelectionVisuals();
    }

    private void HandleThumbstickNav()
    {
        if (thumbstickCooldown > 0 || listEntries.Count == 0) return;

        Vector2 thumbstick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);

        if (thumbstick.y > 0.5f)
        {
            selectedIndex--;
            if (selectedIndex < 0) selectedIndex = listEntries.Count - 1;
            thumbstickCooldown = COOLDOWN_TIME;
            UpdatePageForSelection();
            UpdateSelectionVisuals();
        }
        else if (thumbstick.y < -0.5f)
        {
            selectedIndex++;
            if (selectedIndex >= listEntries.Count) selectedIndex = 0;
            thumbstickCooldown = COOLDOWN_TIME;
            UpdatePageForSelection();
            UpdateSelectionVisuals();
        }
    }

    private void HandleConfirm()
    {
        if (OVRInput.GetDown(OVRInput.Button.One) && listEntries.Count > 0)
        {
            SelectWaypoint(selectedIndex);
        }
    }

    private void SelectWaypoint(int index)
    {
        if (index < 0 || index >= listEntries.Count) return;

        FreeRoamListEntry entry = listEntries[index];
        string waypointId = entry.WaypointId;
        activeWaypointId = waypointId;

        // Update line connector
        GameObject visualObj = WaypointManager.Instance.GetWaypointVisual(waypointId);
        if (visualObj != null)
        {
            if (lineConnector != null) lineConnector.SetTarget(visualObj.transform);
            if (compass != null) compass.Waypoint = visualObj;
        }

        // Update active indicator
        for (int i = 0; i < listEntries.Count; i++)
        {
            listEntries[i].SetActiveSelect(i == index);
        }
    }

    private void UpdateSelectionVisuals()
    {
        for (int i = 0; i < listEntries.Count; i++)
        {
            listEntries[i].SetHighlighted(i == selectedIndex);
        }
    }

    private void UpdatePageForSelection()
    {
        int targetPage = selectedIndex / itemsPerPage;
        if (targetPage != currentPage)
        {
            currentPage = targetPage;
            UpdatePageDisplay();
        }
    }

    private void UpdatePageDisplay()
    {
        for (int i = 0; i < listEntries.Count; i++)
        {
            int itemPage = i / itemsPerPage;
            listEntries[i].gameObject.SetActive(itemPage == currentPage);
        }
    }
}
