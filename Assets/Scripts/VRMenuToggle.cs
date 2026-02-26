using UnityEngine;
using UnityEngine.InputSystem;

public class VRMenuToggle : MonoBehaviour
{
    [SerializeField] private VRMenu vrMenu;
    [SerializeField] private InputActionProperty toggleMenuAction;

    void OnEnable()
    {
        toggleMenuAction.action.Enable();
        toggleMenuAction.action.started += OnToggleMenu;
    }

    void OnDisable()
    {
        toggleMenuAction.action.started -= OnToggleMenu;
        toggleMenuAction.action.Disable();
    }

    private void OnToggleMenu(InputAction.CallbackContext context)
    {
        vrMenu.ToggleMenu();
    }
}