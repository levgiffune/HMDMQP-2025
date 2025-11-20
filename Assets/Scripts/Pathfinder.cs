using UnityEngine;
using UnityEngine.AI;

public class Pathfinder : MonoBehaviour{
    public GameObject Waypoint;
    public float WaypointHeight = 50f;
    public bool EnableDebug;
    private NavMeshPath path;
    private LineRenderer line;

    private void Start(){
        path = new NavMeshPath();
        line = GetComponent<LineRenderer>();
    }

    private void Update(){
        if(Waypoint == null) return;
        NavMeshHit hit;
        Vector3 snap = transform.position;
        Vector3 target = Waypoint.transform.position;

        if (NavMesh.SamplePosition(snap, out hit, 5f, NavMesh.AllAreas)) {
            snap = hit.position;
        }else{
            Debug.Log("Can't snap user");
            return;
        }

        if (NavMesh.SamplePosition(target, out hit, WaypointHeight, NavMesh.AllAreas)) {
            target = hit.position;
        }else{
            Debug.Log("Can't snap target");
            return;
        }

        if (NavMesh.CalculatePath(snap, target, NavMesh.AllAreas, path)){
            if(EnableDebug){
                for(int i = 0; i < path.corners.Length-1; i++){
                    // Debug.Log($"Path: {path.corners[i]} to {path.corners[i+1]}");
                    Debug.DrawLine(path.corners[i], path.corners[i+1], Color.red, 1.0f, false);
                }
            }

            if(line != null){
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