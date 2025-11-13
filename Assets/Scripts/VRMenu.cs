using UnityEngine;

public class VRMenu : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float distanceFromCamera = 2f;
    [SerializeField] private float heightOffset = 0f;
    [SerializeField] private bool followCamera = true;
    public bool startVisible = false;
    
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    public bool IsOpen => canvas != null && canvas.enabled;
    
    void Start()
    {
        canvas = GetComponent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        // Start hidden
        SetMenuActive(startVisible);
        
        // Get main camera if not assigned
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void LateUpdate()
    {
        if (followCamera && canvas.enabled)
        {
            PositionMenuInFrontOfCamera();
        }
    }

    void PositionMenuInFrontOfCamera()
    {
        // Position in front of camera
        Vector3 targetPosition = cameraTransform.position + 
            cameraTransform.forward * distanceFromCamera + 
            Vector3.up * heightOffset;
        
        transform.position = targetPosition;
        
        // Make it face the camera
        transform.LookAt(cameraTransform);
        transform.Rotate(0, 180, 0); // Flip to face user
    }

    public void ToggleMenu()
    {
        SetMenuActive(!canvas.enabled);
    }

    public void SetMenuActive(bool active)
    {
        canvas.enabled = active;
        canvasGroup.interactable = active;
        canvasGroup.blocksRaycasts = active;
        
        if (active)
        {
            PositionMenuInFrontOfCamera();
        }
    }
}