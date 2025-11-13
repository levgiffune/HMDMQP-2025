using UnityEngine;
using TMPro;

public class WaypointCustom : MonoBehaviour
{
    private Renderer rend;
    public TMP_Text nameLabel; // assign in Inspector or dynamically

    void Awake()
    {
        rend = GetComponent<Renderer>();

        // Initialize label text
        if (nameLabel != null)
            nameLabel.text = gameObject.name;
    }

    void Update()
    {
        // Make the label always face the camera
        if (nameLabel != null && Camera.main != null)
        {
            Vector3 direction = nameLabel.transform.position - Camera.main.transform.position;
            if (direction != Vector3.zero)
                nameLabel.transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    public void SetColor(Color newColor)
    {
        if (rend != null)
            rend.material.color = newColor;
    }

    public void SetName(string newName)
    {
        gameObject.name = newName;

        if (nameLabel != null)
            nameLabel.text = newName;
    }
}
