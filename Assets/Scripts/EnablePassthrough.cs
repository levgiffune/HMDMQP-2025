using UnityEngine;

public class EnablePassthrough : MonoBehaviour
{
    void Start()
    {
        OVRManager.instance.isInsightPassthroughEnabled = true;
    }
}