using UnityEngine;
using UnityEngine.AI;

public class Pathfinder : MonoBehaviour{
    public GameObject Waypoint;
    public NavMeshPath Path;
    private float time = 0.0f;

    void Start(){
        Path = new NavMeshPath();
        time = 0.0f;
    }

    void Update(){
        time += Time.deltaTime;
        if(time > 1.0f){
            time -= 1.0f;
            NavMesh.CalculatePath(transform.position, Waypoint.transform.position, NavMesh.AllAreas, Path);
        }

        for(int i = 0; i < Path.corners.Length-1; i++){
            Debug.DrawLine(Path.corners[i], Path.corners[i+1], Color.red, 3.0f, false);
        }
    }
}