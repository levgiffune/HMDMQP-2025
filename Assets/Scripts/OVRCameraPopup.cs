using UnityEngine;

public class OVRCameraPopup : MonoBehaviour
{
    [Header("References")]
    public Transform centerEye; // Assign the CenterEyeAnchor from your OVR Camera Rig

    [Header("Settings")]
    public float distanceFromCamera = 0.5f;  // How far in front of the eyes
    public float followSpeed = 10f;          // Smooth movement speed

    void Start()
    {
        if (centerEye == null)
        {
            // Auto-find the CenterEyeAnchor if not assigned
            OVRCameraRig rig = FindObjectOfType<OVRCameraRig>();
            if (rig != null)
                centerEye = rig.centerEyeAnchor;
            else
                Debug.LogWarning("No OVRCameraRig found in the scene!");
        }
    }

    void Update()
    {
        if (centerEye == null) return;

        // Target position in front of the eyes
        Vector3 targetPosition = centerEye.position + centerEye.forward * distanceFromCamera;

        // Smoothly move the popup
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        // Rotate to always face the user
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            Quaternion.LookRotation(transform.position - centerEye.position),
            followSpeed * Time.deltaTime
        );
    }

    // Call this from your X button
    public void ClosePopup()
    {
        gameObject.SetActive(false);
    }
}
