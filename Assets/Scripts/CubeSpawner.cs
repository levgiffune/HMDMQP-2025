using UnityEngine;
using UnityEngine.InputSystem;

public class CubeSpawner : MonoBehaviour
{
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float spawnDistance = 0.5f;
    [SerializeField] private InputActionProperty spawnAction;
    [SerializeField] private Vector3 cubeSize = new Vector3(0.05f, 0.05f, 0.05f);
    [SerializeField] private VRMenu vrMenu;

    void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main.transform;
        }
    }
    
    void OnEnable()
    {
        spawnAction.action.Enable();
        spawnAction.action.performed += OnSpawnCube;
    }
    
    void OnDisable()
    {
        spawnAction.action.performed -= OnSpawnCube;
        spawnAction.action.Disable();
    }
    
    private void OnSpawnCube(InputAction.CallbackContext context)
    {
        if (vrMenu.IsOpen) {
            Vector3 spawnPosition = playerCamera.position + playerCamera.forward * spawnDistance;
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = spawnPosition;
            cube.transform.localScale = cubeSize;
        }
    }
}