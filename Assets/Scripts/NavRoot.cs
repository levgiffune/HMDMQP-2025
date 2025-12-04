using UnityEngine;

public class NavRoot : MonoBehaviour{
    public Transform Headset;
    public float floor = 0.0f;
    public bool TrackHeadset;

    private void LateUpdate(){
        if (Headset == null) return;
        Vector3 pos;
        if(TrackHeadset){
            pos = Headset.position;
        }else
        {
            pos = transform.position;
        }

        pos.y = floor;

        transform.position = pos;
        transform.rotation = Quaternion.identity;
        // Debug.Log($"pos={pos}");
    }
}