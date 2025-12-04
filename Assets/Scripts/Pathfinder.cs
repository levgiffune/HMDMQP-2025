using UnityEngine;
using UnityEngine.AI;

public class Pathfinder : MonoBehaviour{
    public static Pathfinder Instance;
    public Transform Headset;
    public GameObject Waypoint;
    public float WaypointHeight = 50f;
    public float HeadsetHeight = 50f;
    public bool EnableDebug;
    public bool EnableDebugLine;
    private NavMeshPath path;
    private LineRenderer line;

        private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start(){
        path = new NavMeshPath();
        line = GetComponent<LineRenderer>();
    }

    private void LateUpdate(){
        if(Waypoint == null) return;
        if(Headset == null) return;
        NavMeshHit hit;
        Vector3 snap = Headset.position;
        Vector3 target = Waypoint.transform.position;

        if (NavMesh.SamplePosition(snap, out hit, HeadsetHeight, NavMesh.AllAreas)) {
            snap = hit.position;
        }else{
            if(EnableDebug) Debug.Log("Can't snap user");
            return;
        }

        if (NavMesh.SamplePosition(target, out hit, WaypointHeight, NavMesh.AllAreas)) {
            target = hit.position;
        }else{
            if(EnableDebug) Debug.Log("Can't snap target");
            return;
        }

        if (NavMesh.CalculatePath(snap, target, NavMesh.AllAreas, path)){
            if(EnableDebugLine){
                for(int i = 0; i < path.corners.Length-1; i++){
                    // Debug.Log($"Path: {path.corners[i]} to {path.corners[i+1]}");
                    Debug.DrawLine(path.corners[i], path.corners[i+1], Color.red, 1.0f, false);
                }
            }

            if(line != null){
                // line.useWorldSpace = true;
                line.positionCount = path.corners.Length;
                line.SetPositions(path.corners);
            }
        }
        else
        {
            Debug.Log(path.status);
        }
    }
}