using UnityEngine;

public class SelectionIndicator : MonoBehaviour
{
    public float rotationSpeed = 30f;
    public float pulseSpeed = 2f;
    public float pulseAmount = 0.1f;
    
    private Vector3 baseScale;
    
    private void Awake()
    {
        baseScale = transform.localScale;
    }
    
    private void Update()
    {
        // Slow rotation
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        
        // Gentle pulse
        float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        transform.localScale = baseScale * pulse;
    }
}