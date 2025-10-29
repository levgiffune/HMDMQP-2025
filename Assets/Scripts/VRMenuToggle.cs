using UnityEngine;
using UnityEngine.InputSystem;

public class VRMenuToggle : MonoBehaviour
{
    [SerializeField] private VRMenu vrMenu;
    [SerializeField] private InputActionProperty toggleMenuAction;

    void OnEnable()
    {
        toggleMenuAction.action.Enable();
        toggleMenuAction.action.performed += OnToggleMenu;
    }

    void OnDisable()
    {
        toggleMenuAction.action.performed -= OnToggleMenu;
        toggleMenuAction.action.Disable();
    }

    private void OnToggleMenu(InputAction.CallbackContext context)
    {
        vrMenu.ToggleMenu();
    }
}