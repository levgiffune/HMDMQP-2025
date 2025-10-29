using UnityEngine;
using UnityEngine.UI;

public class VRMenuButtons : MonoBehaviour
{
    [SerializeField] private Button addButton;

    void Start()
    {
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
    
    private void OnStartButtonPressed()
    {
        
    }
    
}