using UnityEngine;

public class PopupController : MonoBehaviour
{
    public Canvas canvas;
    public Transform playerCamera;
    private bool isVisible = true;


    void Start()
    {
        
        if (playerCamera == null)
        {
            playerCamera = Camera.main.transform;
        }
    }

    void LateUpdate()
    {
        HandleToggle();
        
        if (!isVisible) return;
    }

    void HandleToggle()
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick))
        {
            isVisible = !isVisible;
            canvas.enabled = isVisible;
        }
    }
}
