using UnityEngine;
using UnityEngine.UI;

public class VRMenuButtons : MonoBehaviour
{
    [SerializeField] private Button addButton;
    [SerializeField] private Transform playerCamera; // Assign your camera/head transform
    [SerializeField] private float spawnDistance = 0.5f;
    [SerializeField] private Vector3 cubeSize = new Vector3(0.15f, 0.15f, 0.15f);

    void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main.transform;
        }
        // Add listeners to buttons
        if (addButton != null)
            addButton.onClick.AddListener(OnAddButtonPressed);
    }
    
    void OnDestroy()
    {
        // Clean up listeners
        if (addButton != null)
            addButton.onClick.RemoveListener(OnAddButtonPressed);
    }
    
    private void OnAddButtonPressed()
    {
        // Calculate position in front of player
        Vector3 spawnPosition = playerCamera.position + playerCamera.forward * spawnDistance;
        
        // Create cube
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = spawnPosition;
        cube.transform.localScale = Vector3.one * 0.3f; // 30cm cube
        
        // Add a color
        Renderer renderer = cube.GetComponent<Renderer>();
        renderer.material.color = Random.ColorHSV();
    }
}