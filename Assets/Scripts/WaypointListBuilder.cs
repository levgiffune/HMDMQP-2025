using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class WaypointListBuilder : MonoBehaviour
{
    public List<Waypoint> Waypoints;
    public string WaypointFileName;
    public TextAsset jsonFileDev;

    public static WaypointListBuilder Instance { get; private set; }
    private void SaveTextToFile(string fileName, string content){
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        try{
            File.WriteAllText(filePath, content);
            Debug.Log($"Successfully wrote to file: {filePath}");
        }
        catch (System.Exception e){
            Debug.LogError($"Error writing to file {filePath}: {e.Message}");
        }
    }

    private string LoadTextFromFile(string fileName){
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        string content = "";

        if (File.Exists(filePath)){
            content = File.ReadAllText(filePath);
            Debug.Log($"Successfully read from file: {filePath}");
        }
        else {
            Debug.LogWarning($"File not found: {filePath}");
        }
        return content;
    }

    private WaypointList LoadWaypoints(string json)
    {
        WaypointList jsonIn = JsonUtility.FromJson<WaypointList>(json);

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
        return jsonIn;
    }

    public void SaveWaypoints(List<Waypoint> wp)
    {
        WaypointList w = new WaypointList {Waypoints = wp};
        SaveTextToFile(WaypointFileName, JsonUtility.ToJson(w));
    }

    void Start()
    {
        if (WaypointManager.Instance == null)
        {
            Debug.LogError("WaypointListBuilder: WaypointManager.Instance not ready.");
            return;
        }
        string json = LoadTextFromFile(WaypointFileName);
        if(json != "")
        {
            LoadWaypoints(json);
        }
    }

    void Update()
    {
        if(jsonFileDev)
        {
            LoadWaypoints(jsonFileDev.text);
            SaveWaypoints(Waypoints);
            jsonFileDev = null;
        }
    }
}

