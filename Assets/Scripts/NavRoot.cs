using UnityEngine;

public class NavRoot : MonoBehaviour{
    public Transform Headset;
    public float floor = 0.0f;

    private void LateUpdate(){
        if (Headset == null) return;
        Vector3 pos = Headset.position;
        pos.y = floor;

        transform.position = pos;
        Debug.Log($"pos={pos}");
    }
}