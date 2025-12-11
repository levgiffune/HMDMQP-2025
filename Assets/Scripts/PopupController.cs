using UnityEngine;

public class PopupController : MonoBehaviour
{
    public Canvas canvas;
    private bool isVisible = true;

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
