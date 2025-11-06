using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class CompassManager : MonoBehaviour
{
    public static CompassManager Instance;
    public RawImage CompassImage;

    private Transform xrHeadTransform;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        // Try to find the XR camera tagged as "MainCamera"
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            xrHeadTransform = mainCam.transform;
        }
    }

    private void LateUpdate() => UpdateCompassHeading();

    private void UpdateCompassHeading()
    {
        if (xrHeadTransform == null)
        {
            // Try to reassign if not found yet
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                xrHeadTransform = mainCam.transform;
            }
            else
            {
                return;
            }
        }

        float yaw = xrHeadTransform.rotation.eulerAngles.y;
        Vector2 compassUvPosition = Vector2.right * (yaw / 360f);
        CompassImage.uvRect = new Rect(compassUvPosition, Vector2.one);
    }
}