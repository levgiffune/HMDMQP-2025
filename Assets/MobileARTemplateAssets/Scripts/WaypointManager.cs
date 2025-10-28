using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    private Renderer rend;

    void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    // Color Options
    public void SetColorRed()
    {
        rend.material.color = Color.red;
    }

    public void SetColorGreen()
    {
        rend.material.color = Color.green;
    }

    public void SetColorBlue()
    {
        rend.material.color = Color.blue;
    }
}
