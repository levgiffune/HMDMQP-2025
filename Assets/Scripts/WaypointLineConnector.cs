using UnityEngine;

public class WaypointLineConnector : MonoBehaviour
{
    public static WaypointLineConnector Instance { get; private set; }

    [Header("References")]
    public Transform cameraTransform;
    public LineRenderer lineRenderer;

    [Header("Settings")]
    public float startHeightOffset = 0.5f;

    private Transform targetWaypoint;

    void Start()
    {
        lineRenderer.positionCount = 0;
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (targetWaypoint == null || lineRenderer == null)
        {
            return;
        }

        Vector3 start = cameraTransform.position - new Vector3(0, startHeightOffset, 0);
        Vector3 end = targetWaypoint.position;

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }

    public void SetTarget(Transform waypoint)
    {
        targetWaypoint = waypoint;
    }

    public void ClearTarget()
    {
        targetWaypoint = null;
        lineRenderer.positionCount = 0;
    }
}