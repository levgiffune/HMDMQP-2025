using UnityEngine;

public class ControllerRay : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public float rayLength = 5f;
    
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }
    
    void Update()
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position + transform.forward * rayLength);
    }
}