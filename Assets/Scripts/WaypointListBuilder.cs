using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class WaypointListBuilder : MonoBehaviour
{
    public List<Waypoint> Waypoints;
    public TextAsset jsonFile;

    void Start()
    {
        if (WaypointManager.Instance == null)
        {
            Debug.LogError("WaypointListBuilder: WaypointManager.Instance not ready.");
            return;
        }

        WaypointList jsonIn = JsonUtility.FromJson<WaypointList>(jsonFile.text);

        Waypoints.AddRange(jsonIn.Waypoints);

        Debug.Log($"WaypointListBuilder: Loaded {Waypoints.Count} waypoints from JSON.");

        foreach (Waypoint w in Waypoints)
        {
            // Load images from Resources by name
            if (w.imageNames != null && w.imageNames.Length > 0)
            {
                w.images = w.imageNames
                    .Select(name => Resources.Load<Texture2D>(name))
                    .Where(tex => tex != null)
                    .ToArray();

                int failed = w.imageNames.Length - w.images.Length;
                if (failed > 0)
                {
                    Debug.LogWarning($"WaypointListBuilder: {failed} image(s) not found in Resources for waypoint '{w.name}'.");
                }
            }

            WaypointManager.Instance.CreateWaypoint(w);
        }
    }
}

