using UnityEngine;

public class Waypoint : MonoBehaviour
{
    private Renderer rend;

    void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    public void SetColor(Color newColor)
    {
        rend.material.color = newColor;
    }
}
