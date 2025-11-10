using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class CompassManager : MonoBehaviour
{
    public static CompassManager Instance;
    public RawImage CompassImage;
    public RawImage WaypointMarker;
    public GameObject Waypoint;
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

        if (Waypoint == null)
        {
            WaypointMarker.gameObject.SetActive(false);
            return;
        }

        if (!WaypointMarker.gameObject.active)
        {
            WaypointMarker.gameObject.SetActive(true);
        }

        Transform WTransform = Waypoint.transform;
        Transform PTransform = xrHeadTransform.transform;

        Vector3 cast = WTransform.position - PTransform.position;
        // Vector3 PEyes = PTransform.forward;

        cast.y = 0;
        // PEyes.y = 0;

        float WaypointAngle = Quaternion.LookRotation(cast).eulerAngles.y;
        float delta = Mathf.DeltaAngle(yaw, WaypointAngle);

        Debug.Log($"Waypoint angle delta: {delta}, Waypoint angle: {WaypointAngle}");

        RectTransform WaypointTransform = WaypointMarker.GetComponent<RectTransform>();
        WaypointTransform.anchoredPosition = new Vector2(
            1024f*delta/360f,
            WaypointTransform.anchoredPosition.y);
    }
}